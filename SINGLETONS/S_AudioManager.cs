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
    private IntPropertySO currLives = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;
    public PSO_CurrentGameMode CurrGameMode => currGameMode;

    //getters for the zone PSO and the pause delegate so that audio wrappers can access them
    //and so we don't have to waste a ton of time setting those if we know
    //they will all be the same for all of the wrappers. Doing the pause
    //logic here for also keeps it in one place and saves computation.
    [SerializeField]
    private IntPropertySO currZone = null;
    public IntPropertySO CurrZone => currZone;

    private bool cachedPausedToggle = false;

    private Dictionary<AudioWrapperSource, List<Coroutine>> soundCRs = new Dictionary<AudioWrapperSource, List<Coroutine>>();
    private Dictionary<AudioWrapperSource, List<Coroutine>> unstoppableSoundCRs = new Dictionary<AudioWrapperSource, List<Coroutine>>();

    public event System.Action<bool> PauseToggleAllAudioClipWrapperV2s = delegate { };

    protected override void OnAwake()
    {
        currLives.RegisterForPropertyChanged(OnCurrLivesChange);
        currGameMode.RegisterForPropertyChanged(OnGameModeChanged);
    }


    //TODO: take out once scene switching is in place
    private void Start()
    {
        OnSceneChange(GameModeEnum.PLAY);
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
        if (newMode == GameModeEnum.PAUSE)
        {
            PauseToggleAllAudioClipWrapperV2s(true);
            cachedPausedToggle = true;
        }
        else if (prevMode == GameModeEnum.PAUSE && newMode == GameModeEnum.PLAY)
        {
            PauseToggleAllAudioClipWrapperV2s(false);
            cachedPausedToggle = false;
        }
    }

    //TODO: link with scene manager once in place
    private void OnSceneChange(GameModeEnum newMode)
    {
        StopAllDelayedSounds(soundCRs);
        StopAllDelayedSounds(unstoppableSoundCRs);
        soundCRs.Clear();
        unstoppableSoundCRs.Clear();

        loopSettings.SpawnAndPlayNewLoopObjectSource(newMode);
    }

    private void StopAllDelayedSounds(Dictionary<AudioWrapperSource, List<Coroutine>> soundCRDict)
    {
        foreach (AudioWrapperSource key in soundCRDict.Keys)
        {
            foreach (Coroutine cr in soundCRDict[key])
            {
                StopCoroutine(cr);
            }
        }
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
        Coroutine newCR = null;
        newCR = StartCoroutine(PlayDelayedCR(aw, del, soundObject, newCR));

        Dictionary<AudioWrapperSource, List<Coroutine>> soundCRDict = unstoppable ? unstoppableSoundCRs : soundCRs;

        if (!soundCRDict.ContainsKey(soundObject))
        {
            soundCRDict.Add(soundObject, new List<Coroutine>());
        }

        soundCRDict[soundObject].Add(newCR);
    }

    /// <summary>
    /// Coroutine used by PlayDelayed to play an AudioWrapper after a delay
    /// </summary>
    /// <param name="aw">AudioWrapper to be played</param>
    /// <param name="delay"Time to delay before playing in seconds></param>
    /// <param name="soundObject">AudioWrapperSource to play the AudioWrapper from</param>
    /// <returns></returns>
    private IEnumerator PlayDelayedCR(AAudioWrapperV2 aw, float delay, AudioWrapperSource soundObject, Coroutine selfCR)
    {
        while (!cachedPausedToggle)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        if (soundObject != null)
        {
            aw.PlayAudioWrapper(soundObject);
            RemoveFromSoundCRDict(soundCRs, soundObject, selfCR);
            RemoveFromSoundCRDict(unstoppableSoundCRs, soundObject, selfCR);
        }
    }

    private void RemoveFromSoundCRDict(Dictionary<AudioWrapperSource, List<Coroutine>> soundCRDict, AudioWrapperSource soundObject, Coroutine cr)
    {
        if (!soundCRDict.ContainsKey(soundObject))
        {
            return;
        }

        soundCRDict[soundObject].Remove(cr);

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
        //TODO: is this expensive to add and remove so frequently? Cause
        //we don't want to have deleted object keys chilling in there
        //as long as a player hasn't lost a run...

        if (soundCRs.TryGetValue(obj, out var crList))
        {
            foreach (Coroutine cr in crList)
            {
                StopCoroutine(cr);
            }
            soundCRs.Remove(obj);
        }
    }
}
