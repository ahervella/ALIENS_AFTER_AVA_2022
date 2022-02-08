using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static AudioUtil;

public class S_AudioManager : Singleton<S_AudioManager>
{
    [SerializeField]
    private List<SO_LoopAudioSettings> loopSettings = new List<SO_LoopAudioSettings>();

    [SerializeField]
    private List<SO_MixerEffectWrapper> mixerEffectWrappers = new List<SO_MixerEffectWrapper>();

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

    private Dictionary<GameObject, List<Coroutine>> soundCRs = new Dictionary<GameObject, List<Coroutine>>();
    private Dictionary<GameObject, List<Coroutine>> unstoppableSoundCRs = new Dictionary<GameObject, List<Coroutine>>();

    public event System.Action<bool> PauseToggleAllAudioClipWrapperV2s = delegate { };

    protected override void OnAwake()
    {
        currLives.RegisterForPropertyChanged(OnCurrLivesChange);
        currGameMode.RegisterForPropertyChanged(OnGameModeChanged);
    }

    private void OnCurrLivesChange(int oldLives, int newLives)
    {
        switch (newLives)
        {
            case 0:
                GetMixerEffectSetting(MixerEffectEnum.LAST_LIFE).SetAsCurrentSnapshot();
                return;

            case -1:
                GetMixerEffectSetting(MixerEffectEnum.GAME_OVER).SetAsCurrentSnapshot();
                return;

            default:
                GetMixerEffectSetting(MixerEffectEnum.DEFAULT_LIFE).SetAsCurrentSnapshot();
                return;
        }
    }

    private void OnGameModeChanged(GameModeEnum prevMode, GameModeEnum newMode)
    {
        switch (newMode)
        {
            case GameModeEnum.PAUSE:
                PauseToggleAllAudioClipWrapperV2s(true);
                cachedPausedToggle = true;
                return;
            case GameModeEnum.PLAY:
                if (prevMode == GameModeEnum.PAUSE)
                {
                    PauseToggleAllAudioClipWrapperV2s(false);
                    cachedPausedToggle = false;
                }
                else if (prevMode == GameModeEnum.MAINMENU)
                {
                    CleanUpForSceneChange();
                }
                return;
            case GameModeEnum.EXIT:
            case GameModeEnum.MAINMENU:
                CleanUpForSceneChange();
                return;
        }
    }

    private void CleanUpForSceneChange()
    {
        StopAllDelayedSounds(soundCRs);
        StopAllDelayedSounds(unstoppableSoundCRs);
        soundCRs.Clear();
        unstoppableSoundCRs.Clear();
    }

    private void StopAllDelayedSounds(Dictionary<GameObject, List<Coroutine>> soundCRDict)
    {
        foreach (GameObject key in soundCRDict.Keys)
        {
            foreach (Coroutine cr in soundCRDict[key])
            {
                StopCoroutine(cr);
            }
        }
    }

    private void Start()
    {
        GetMixerEffectSetting(MixerEffectEnum.DEFAULT_LIFE).SetAsCurrentSnapshot();
        StartLoopedAudio();
    }

    /// <summary>
    /// Gets the MixerEffectsSettings that matches the mixer type
    /// </summary>
    /// <param name="mixerType">mixer type to match</param>
    /// <returns></returns>
    private SO_MixerEffectWrapper GetMixerEffectSetting(MixerEffectEnum mixerType)
    {
        foreach (var mix in mixerEffectWrappers)
        {
            if (mix.MixerType == mixerType)
            {
                return mix;
            }
        }
        Debug.LogError("You suck there's no mixer for this mixer scenario type: " + mixerType);
        return null;
    }

    private void StartLoopedAudio()
    {
        foreach(SO_LoopAudioSettings ls in loopSettings)
        {
            if (ls.GameMode == currGameMode.Value)
            {
                ls.PlayAllACWs(gameObject);
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
    public void PlayDelayed(AAudioWrapperV2 aw, float del, GameObject soundObject, AudioMixerGroup mixerGroup, bool unstoppable)
    {
        Coroutine newCR = null;
        newCR = StartCoroutine(PlayDelayedCR(aw, del, soundObject, mixerGroup, newCR));

        Dictionary<GameObject, List<Coroutine>> soundCRDict = unstoppable ? unstoppableSoundCRs : soundCRs;

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
    /// <param name="del"Time to delay before playing in seconds></param>
    /// <param name="soundObject">GameObject to play the AudioWrapper from</param>
    /// <returns></returns>
    private IEnumerator PlayDelayedCR(AAudioWrapperV2 aw, float del, GameObject soundObject, AudioMixerGroup mixerGroup, Coroutine selfCR)
    {
        while (!cachedPausedToggle)
        {
            yield return new WaitForSecondsRealtime(del);
        }

        if (soundObject != null)
        {
            aw.PlayAudioWrapper(soundObject, mixerGroup);
            RemoveFromSoundCRDict(soundCRs, soundObject, selfCR);
            RemoveFromSoundCRDict(unstoppableSoundCRs, soundObject, selfCR);
        }
    }

    private void RemoveFromSoundCRDict(Dictionary<GameObject, List<Coroutine>> soundCRDict, GameObject soundObject, Coroutine cr)
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
    public void StopAllDelayedSounds(GameObject obj)
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
