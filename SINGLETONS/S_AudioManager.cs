using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static AudioUtil;

public class S_AudioManager : Singleton<S_AudioManager>
{
    [SerializeField]
    private SO_MixerEffectSettings mixerSettings = null;

    [SerializeField]
    private SO_LoopAudioSettings loopSettings = null;

    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    [SerializeField]
    private IntPropertySO currLives = null;

    [SerializeField]
    private float maxAudioTileDist = 20;

    public float CachedAudioDist { get; private set; }

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private PSO_CurrentZonePhase currZonePhase = null;

    //getters for the zone PSO and the pause delegate so that audio wrappers can access them
    //and so we don't have to waste a ton of time setting those if we know
    //they will all be the same for all of the wrappers. Doing the pause
    //logic here for also keeps it in one place and saves computation.
    [SerializeField]
    private IntPropertySO currZone = null;
    public IntPropertySO CurrZone => currZone;

    private bool cachedPausedToggle = false;

    private Dictionary<AudioWrapperSource, Dictionary<int, Coroutine>> soundCRs = new Dictionary<AudioWrapperSource, Dictionary<int, Coroutine>>();
    private Dictionary<AudioWrapperSource, Dictionary<int, Coroutine>> unstoppableSoundCRs = new Dictionary<AudioWrapperSource, Dictionary<int, Coroutine>>();

    public event System.Action<bool> PauseToggleAllAudioClipWrapperV2s = delegate { };

    private int randUniqueInt = 0;

    private Dictionary<Tuple<GameModeEnum, ZonePhaseEnum, int>, GameObject> loopedAudioDict = new Dictionary<Tuple<GameModeEnum, ZonePhaseEnum, int>, GameObject>();

    private LoopAudioWrapper currLoopAudioWrapper = null;

    protected override void OnAwake()
    {
        currLives.RegisterForPropertyChanged(OnCurrLivesChange, persistent);
        currGameMode.RegisterForPropertyChanged(OnGameModeChanged, persistent);
        currZonePhase.RegisterForPropertyChanged(OnZonePhaseChange, persistent);
        currZone.RegisterForPropertyChanged(OnZoneChange, persistent);

        CachedAudioDist = terrSettings.TileDims.y * maxAudioTileDist;

        //Can't use start because would trigger it every time we change scene
        StartCoroutine(StartCR());
    }

    private IEnumerator StartCR()
    {
        yield return null;
        OnGameModeChanged(currGameMode.Value, currGameMode.Value);
        OnCurrLivesChange(-1, currLives.Value);
    }

    private void OnCurrLivesChange(int oldLives, int newLives)
    {
        switch (newLives)
        {
            case 0:
                mixerSettings.SetMixerEffectSnapshot(MixerEffectEnum.LAST_LIFE);
                return;

            case -1:
                mixerSettings.SetMixerEffectSnapshot(MixerEffectEnum.GAME_OVER);
                return;

            default:
                mixerSettings.SetMixerEffectSnapshot(MixerEffectEnum.DEFAULT_LIFE);
                return;
        }
    }

    private void OnGameModeChanged(GameModeEnum prevMode, GameModeEnum newMode)
    {
        //TODO: test pausing is working correctly
        if (newMode == GameModeEnum.PAUSE)
        {
            PauseToggleAllAudioClipWrapperV2s(true);
            cachedPausedToggle = true;
            return;
        }
        else if (prevMode == GameModeEnum.PAUSE)
        {
            PauseToggleAllAudioClipWrapperV2s(false);
            cachedPausedToggle = false;

            //if unpaused, and still running, dont reset audio in PlayGameModeAudioLoop
            if (newMode == GameModeEnum.PLAY) { return; }
        }

        if (newMode == GameModeEnum.MAINMENU)
        {
            mixerSettings.SetMixerEffectSnapshot(MixerEffectEnum.DEFAULT_LIFE);
        }

        UpdateAndPlayAudioLoop();
    }

    private void OnZonePhaseChange(ZonePhaseEnum _, ZonePhaseEnum __)
    {
        UpdateAndPlayAudioLoop();
    }

    private void OnZoneChange(int _, int __)
    {
        UpdateAndPlayAudioLoop();
    }

    private void UpdateAndPlayAudioLoop()
    {
        GameModeEnum mode = currGameMode.Value;
        ZonePhaseEnum phase = currZonePhase.Value;
        int zone = currZone.Value;

        LoopAudioWrapper loopAW = loopSettings.GetAudioLoopWrapper(mode, phase, zone);

        if (loopAW == null || currLoopAudioWrapper == loopAW) { return; }

        StopAllDelayedSounds(soundCRs);
        StopAllDelayedSounds(unstoppableSoundCRs);
        soundCRs.Clear();
        unstoppableSoundCRs.Clear();

        mode = loopAW.GameMode;
        phase = loopAW.ZonePhase;
        zone = loopAW.Zone;

        Tuple<GameModeEnum, ZonePhaseEnum, int> dictKey
            = new Tuple<GameModeEnum, ZonePhaseEnum, int>(mode, phase, zone);

        
        if (!loopedAudioDict.ContainsKey(dictKey))
        {
            GameObject newObj = new GameObject($"LOOPED_AUDIO-{mode}");
            newObj.transform.parent = transform;
            AudioWrapperSource aws = newObj.AddComponent<AudioWrapperSource>();
            aws.SetMixerGroup(loopAW.MixerGroup);

            loopedAudioDict.Add(dictKey, newObj);
        }

        foreach(KeyValuePair<Tuple<GameModeEnum, ZonePhaseEnum, int>, GameObject> kvp in loopedAudioDict)
        {
            AudioWrapperSource aws = kvp.Value.GetComponent<AudioWrapperSource>();

            if (kvp.Key.Equals(dictKey))
            {
                currLoopAudioWrapper = loopAW;
                if (!currLoopAudioWrapper.SilenceForThisConfig)
                {
                    StartCoroutine(FadeAudioCR(loopAW.AudioWrapper, aws, true, loopAW.FadeAudioInTime));
                }
                continue;
            }

            LoopAudioWrapper otherLoopAW = loopSettings.GetAudioLoopWrapper(kvp.Key.Item1, kvp.Key.Item2, kvp.Key.Item3);
            if (!otherLoopAW.SilenceForThisConfig)
            {
                StartCoroutine(FadeAudioCR(otherLoopAW.AudioWrapper, aws, false, otherLoopAW.FadeAudioOutTime));
            }
        }
    }

    private void StopAllDelayedSounds(Dictionary<AudioWrapperSource, Dictionary<int, Coroutine>> soundCRDict)
    {
        foreach (AudioWrapperSource key in soundCRDict.Keys)
        {
            foreach (KeyValuePair<int, Coroutine> kvp in soundCRDict[key])
            {
                StopCoroutine(soundCRDict[key][kvp.Key]);
            }
        }
    }

    /// <summary>
    /// Returns whether there are still unstoppable audio sources to be played
    /// </summary>
    /// <param name="aws">The audio source to check</param>
    /// <returns></returns>
    public bool AudioSourceHasAudioQueued(AudioWrapperSource aws)
    {
        //TODO: reconsider whether we need to keep track of stoppable vs unstoppable sounds anymore
        return unstoppableSoundCRs.ContainsKey(aws) || soundCRs.ContainsKey(aws);
    }

    /// <summary>
    /// Starts a coroutine to play an AudioWrapper after a delay time,
    /// will add that coroutine to a list on the object if the wrapper is "stoppable"
    /// </summary>
    /// <param name="aw">AudioWrapper to be played</param>
    /// <param name="del">Time to delay before playing in seconds</param>
    /// <param name="soundObject">GameObject to play the AudioWrapper from</param>
    /// <param name="unstoppable">Prevents this AudioWrapper and its children from being stopped by another AudioWrapper being played</param>
    public void PlayDelayed(AAudioWrapperV2 aw, float del, AudioWrapperSource soundObject, bool unstoppable)
    {
        //TODO: realistically need to worry about this being too large? lol
        int newCRID = randUniqueInt++;

        Coroutine newCR = StartCoroutine(PlayDelayedCR(aw, del, soundObject, newCRID));

        Dictionary<AudioWrapperSource, Dictionary<int, Coroutine>> soundCRDict = unstoppable ? unstoppableSoundCRs : soundCRs;

        if (!soundCRDict.ContainsKey(soundObject))
        {
            soundCRDict.Add(soundObject, new Dictionary<int, Coroutine>());
        }

        soundCRDict[soundObject].Add(newCRID, newCR);
    }

    /// <summary>
    /// Coroutine used by PlayDelayed to play an AudioWrapper after a delay
    /// </summary>
    /// <param name="aw">AudioWrapper to be played</param>
    /// <param name="delay"Time to delay before playing in seconds></param>
    /// <param name="soundObject">AudioWrapperSource to play the AudioWrapper from</param>
    /// <returns></returns>
    private IEnumerator PlayDelayedCR(AAudioWrapperV2 aw, float delay, AudioWrapperSource soundObject, int crID)
    {
        //TODO: test pausing is working correctly
        while (cachedPausedToggle)
        {
            yield return null;
        }

        yield return new WaitForSeconds(delay);

        if (soundObject != null)
        {
            aw.PlayAudioWrapper(soundObject);
            RemoveFromSoundCRDict(soundCRs, soundObject, crID);
            RemoveFromSoundCRDict(unstoppableSoundCRs, soundObject, crID);
        }
    }

    private void RemoveFromSoundCRDict(Dictionary<AudioWrapperSource, Dictionary<int, Coroutine>> soundCRDict, AudioWrapperSource soundObject, int crID)
    {
        if (!soundCRDict.ContainsKey(soundObject))
        {
            return;
        }

        soundCRDict[soundObject].Remove(crID);

        if (soundCRDict[soundObject].Count == 0)
        {
            soundCRDict.Remove(soundObject);
        }
    }


    /// <summary>
    /// Stops all coroutines waiting to play an AudioWrapper on
    /// the given object, if they are stoppable
    /// </summary>
    /// <param name="obj">GameObject to stop the coroutines on</param>
    public void StopAllDelayedSounds(AudioWrapperSource obj)
    {
        StopAllDelayedSounds(soundCRs, obj);
    }

    private void StopAllDelayedSounds(Dictionary<AudioWrapperSource, Dictionary<int, Coroutine>> soundCRDict, AudioWrapperSource obj)
    {
        //TODO: is this expensive to add and remove so frequently? Cause
        //we don't want to have deleted object keys chilling in there
        //as long as a player hasn't lost a run...

        if (soundCRDict.TryGetValue(obj, out var crList))
        {
            foreach (KeyValuePair<int, Coroutine> kvp in crList)
            {
                StopCoroutine(kvp.Value);
            }
            soundCRDict.Remove(obj);
        }
    }

    /// <summary>
    /// Stops all sound coroutines for this audio wrapper source,
    /// and removes all references of this object from the audio manager
    /// singleton
    /// </summary>
    /// <param name="source"></param>
    public void OnAudioWrapperSourceDestroyed(AudioWrapperSource source)
    {
        StopAllDelayedSounds(soundCRs, source);
        StopAllDelayedSounds(unstoppableSoundCRs, source);
    }
}
