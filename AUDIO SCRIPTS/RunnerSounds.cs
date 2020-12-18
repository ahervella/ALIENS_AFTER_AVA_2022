using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

/// <summary>
/// The main singleton which manages the audio in the game
/// </summary>
public class RunnerSounds : MonoBehaviour
{
    [Serializable]
    private class HeartbeatSettings
    {
        [SerializeField]
        public GameObject heartbeatSourceObj;

        [SerializeField]
        public AudioClipWrapper heartbeatWrapper;

        [SerializeField, Range(0f, 2f)]
        public float secondToLastLife = 1;

        [SerializeField, Range(0f, 2f)]
        public float heartbeatDelLastLife = 1;

        [SerializeField, Range(0f, 2f)]
        public float heartbeatDelGameOver = 5;
    }

    [Serializable]
    private class MixerEffectsSettings
    {
        [SerializeField]
        public AudioMixerSnapshot amsMuteStart;
        [SerializeField]
        public AudioMixerSnapshot amsMain;
        [SerializeField]
        public AudioMixerSnapshot amsLastLife;
        [SerializeField]
        public AudioMixerSnapshot amsGameOver;

        [SerializeField]
        [Range(0f, 10f)]
        public float startFadeTime = 1;
        [SerializeField]
        [Range(0f, 10f)]
        public float healthyFadeTime = 1;
        [SerializeField]
        [Range(0f, 10f)]
        public float lastLifeFadeTime = 1;
        [SerializeField]
        [Range(0f, 10f)]
        public float gameOverFadeTime = 1;
    }

    [Serializable]
    private class AmbLoop
    {
        [SerializeField]
        public AudioClip audioClip;
        [SerializeField, Range(-60f, 0f)]
        public float volDb = 0;
        [Range(-1200, 1200)]
        public float pitchCents = 0;
    }

    [Serializable]
    private class AmbienceSettings
    {

        [SerializeField]
        public GameObject ambSourceObj;

        [SerializeField]
        public List<AmbLoop> ambLoops = new List<AmbLoop>();
    }

    [Serializable]
    public class MusicLoop
    {
        [SerializeField]
        public AudioClip audioClip;

        [SerializeField, Range(-60f, 0f)]
        public float volDb = 0;

        [SerializeField]
        public AudioMixerGroup Output;
    }

    [Serializable]
    private class MusicSettings
    {
        [SerializeField]
        public GameObject musicSourceObj;

        [SerializeField]
        public List<MusicLoop> musicLoops = new List<MusicLoop>();
    }

    //All these guys show the settings in the inspector
    [SerializeField]
    private HeartbeatSettings heartbeatSettings = new HeartbeatSettings();

    [SerializeField]
    private AmbienceSettings ambienceSettings = new AmbienceSettings();

    [SerializeField]
    private MusicSettings musicSettings = new MusicSettings();

    [SerializeField]
    private MixerEffectsSettings mixerEffectsSettings = new MixerEffectsSettings();

    static public RunnerSounds Current = null;
    private Coroutine currentHeartbeatLoop = null;

    private Dictionary<GameObject, List<Coroutine>> soundCRs = new Dictionary<GameObject, List<Coroutine>>();

    /// <summary>
    /// Singleton code
    /// </summary>
    private void Awake()
    {
        if (Current == null)
        {
            Current = this;
        }
        else if (Current != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Starts Ambience and Music also fades in the audio mixer
    /// </summary>
    private void Start()
    {
        mixerEffectsSettings.amsMain.TransitionTo(mixerEffectsSettings.startFadeTime);
        StartAmbience();
        StartMusic();
    }

    /// <summary>
    /// Plays an AudioClip from an AudioClipWrapper on the specified GameObject
    /// </summary>
    /// <param name="acw">AudioClipWrapper from which a clip will be chosen and played</param>
    /// <param name="soundObject">GameObject to play from</param>
    /// <returns>Returns the AudioClip which was played</returns>
    public AudioClip PlayAudioClipWrapper(AudioClipWrapper acw, GameObject soundObject)
    {
        AudioSource source = GetAudioSource(soundObject);

        SourceProperties sp = soundObject.GetComponent<SourceProperties>();

        if (sp == null)
        {
            Debug.LogError("No SourceProperties Component on the gameobject: " + soundObject.name);
            return null;
        }

        AudioClip clip = null;

        if (acw.isRandom)
        {
            clip = acw.audioClips[Random.Range(0, acw.audioClips.Count - 1)];
            acw.audioClips.Remove(clip);
            acw.audioClips.Add(clip);
        }
        else
        {
            // :/
            Debug.Log(acw.name + " IS NOT RANDOM >>:(");
        }

        float volDb = acw.LevelDb + Random.Range(-acw.volVrtnDb, acw.volVrtnDb);
        float pitchCents = acw.pitchCents + Random.Range(-acw.pitchVrtnCents, acw.pitchVrtnCents);

        AssignSourceProperties(source, volDb, pitchCents, clip);
        source.Play();
        return clip;
    }

    /// <summary>
    /// Plays an AudioWrapper on the specified GameObject
    /// </summary>
    /// <param name="aw">AudioWrapper to be played</param>
    /// <param name="soundObject">GameObject to be played from</param>
    public void PlayAudioWrapper(AAudioWrapper aw, GameObject soundObject)
    {
        if (aw == null || soundObject == null)
        {
            Debug.LogError("AudioWrapper or target Object did not exist");
            return;
        }
        StopAllDelayedSounds(soundObject);
        aw.PlayAudioWrappers(soundObject);
        //Debug.Log("AudioWrapper << " + aw + " >> was played");
    }

    /// <summary>
    /// Starts looping the specified AudioSource
    /// </summary>
    /// <param name="source">Source to be played from</param>
    private static void StartLoop(AudioSource source)
    {
        source.loop = true;
        source.Play();
    }

    /// <summary>
    /// Plays the ambience loops
    /// </summary>
    private void StartAmbience()
    {
        foreach (AmbLoop loop in ambienceSettings.ambLoops)
        {
            AudioSource source = Current.GetAudioSource(ambienceSettings.ambSourceObj);
            AssignSourceProperties(source, loop.volDb, loop.pitchCents, loop.audioClip);
            StartLoop(source);
        }
    }

    /// <summary>
    /// Plays the music loops
    /// </summary>
    private void StartMusic()
    {
        foreach (MusicLoop loop in musicSettings.musicLoops)
        {
            AudioSource source = Current.GetAudioSource(musicSettings.musicSourceObj);
            AssignSourceProperties(source, loop.Output, 0, 100f, loop.volDb, 0, loop.audioClip);
            StartLoop(source);
        }
    }

    /// <summary>
    /// Gets an audio source to use on the specified GameObject, or creates a new one if none is available
    /// </summary>
    /// <param name="obj">GameObject to get and AudioSource From</param>
    /// <returns>An available or new AudioSource</returns>
    private AudioSource GetAudioSource(GameObject obj)
    {
        AudioSource[] audioSources = obj.GetComponents<AudioSource>();

        if (audioSources != null)
        {
            for (int i = 0; i < audioSources.Length; i++) //Checks for available audio sources
            {
                AudioSource thisSource = audioSources[i];

                if (!thisSource.isPlaying)//If it isn't playing
                {
                    return thisSource; //Use this source
                }
            }
        }
        //Debug.Log("<<" + obj.name + ">> required an additional audio source");
        AudioSource addedSource = obj.AddComponent<AudioSource>();
        return addedSource;
    }

    /// <summary>
    /// Assigns some commonly used source properties to the given object
    /// </summary>
    /// <param name="source">AudioSource to be modified</param>
    /// <param name="output">AudioMixerGroup to be assigned</param>
    /// <param name="spatialBlend">Spatial blend value to be assigned</param>
    /// <param name="maxDist">Maximum Distance to be assigned</param>
    /// <param name="volDb">Volume to be assigned in decibels</param>
    /// <param name="pitchCents">Pitch to be assigned in cents</param>
    /// <param name="clip">AudioClip to be assigned</param>
    private void AssignSourceProperties(AudioSource source, AudioMixerGroup output, float spatialBlend, float maxDist, float volDb, float pitchCents, AudioClip clip)
    {
        source.outputAudioMixerGroup = output;
        source.spatialBlend = spatialBlend;
        source.maxDistance = maxDist;
        source.volume = Mathf.Pow(10, volDb / 20);
        source.pitch = Mathf.Pow(2, pitchCents / 1200);
        source.rolloffMode = AudioRolloffMode.Custom;
        source.clip = clip;
    }

    /// <summary>
    /// Assigns some commonly used source properties to the given object,
    /// omits information that could be given by a SourceProperties script on the same GameObject as the target AudioSource
    /// </summary>
    /// <param name="source">AudioSource to be modified</param>
    /// <param name="volDb">Volume to be assigned in decibels</param>
    /// <param name="pitchCents">Pitch to be assigned in cents</param>
    /// <param name="clip">AudioClip to be assigned</param>
    private void AssignSourceProperties(AudioSource source, float volDb, float pitchCents, AudioClip clip)
    {
        SourceProperties sp = source.gameObject.GetComponent<SourceProperties>();
        AssignSourceProperties(source, sp.output, sp.spatialBlend, sp.maxDist, volDb, pitchCents, clip);
    }

    /// <summary>
    /// Starts a coroutine to play an AudioWrapper after a delay time,
    /// will add that coroutine to a list on the object if the wrapper is "stoppable"
    /// </summary>
    /// <param name="aw">AudioWrapper to be played</param>
    /// <param name="del">Time to delay before playing in seconds</param>
    /// <param name="soundObject">GameObject to play the AudioWrapper from</param>
    /// <param name="unstoppable">Prevents this AudioWrapper and its children from being stopped by another AudioWrapper being played</param>
    public void PlayDelayed(AAudioWrapper aw, float del, GameObject soundObject, bool unstoppable)
    {
        Coroutine newCR = StartCoroutine(DelayAndPlay(aw, del, soundObject));

        //If the event is unstoppable, no not add it to the coroutine list
        if (unstoppable)
        {
            return;
        }
        if (soundCRs.TryGetValue(soundObject, out var crList))
        {
            crList.Add(newCR);
        }
        else
        {
            soundCRs.Add(soundObject, new List<Coroutine> { newCR });
        }
    }

    /// <summary>
    /// Coroutine used by PlayDelayed to play an AudioWrapper after a delay
    /// </summary>
    /// <param name="aw">AudioWrapper to be played</param>
    /// <param name="del"Time to delay before playing in seconds></param>
    /// <param name="soundObject">GameObject to play the AudioWrapper from</param>
    /// <returns></returns>
    private IEnumerator DelayAndPlay(AAudioWrapper aw, float del, GameObject soundObject)
    {
        yield return new WaitForSecondsRealtime(del);
        if (soundObject != null)
        {
            aw.PlayAudioWrappers(soundObject);
        }
    }

    /// <summary>
    /// Stops all coroutines waiting to play an AudioWrapper on
    /// the given object, if they are stoppable
    /// </summary>
    /// <param name="obj">GameObject to stop the coroutines on</param>
    public void StopAllDelayedSounds(GameObject obj)
    {
        if (soundCRs.TryGetValue(obj, out var crList))
        {
            foreach (Coroutine cr in crList)
            {
                StopCoroutine(cr);
            }
            //TODO: is this expensive to add and remove so frequently? Cause
            //we don't want to have deleted object keys chilling in there
            //as long as a player hasn't lost a run...
            soundCRs.Remove(obj);
        }
    }

    /// <summary>
    /// Updates various Mixer settings and plays a heartbeat depending on the player's health
    /// </summary>
    public void PlayerHealthUpdate()
    {
        int lives = GetLives();

        switch (lives)
        {
            //case 1:
            //    if (currentHeartbeatLoop != null)
            //    {
            //        StopCoroutine(currentHeartbeatLoop);
            //        currentHeartbeatLoop = StartCoroutine(heartbeatLoop(heartbeatSettings.heartbeatDelGameOver));
            //        _ = StartCoroutine(WaitAndStopHeartbeat(mixerEffectsSettings.gameOverFadeTime));
            //    }
            //    break;

            case 0:
                mixerEffectsSettings.amsLastLife.TransitionTo(mixerEffectsSettings.lastLifeFadeTime);

                if (currentHeartbeatLoop != null)
                {
                    StopCoroutine(currentHeartbeatLoop);
                }
                currentHeartbeatLoop = StartCoroutine(HeartbeatLoop(heartbeatSettings.heartbeatDelLastLife));

                break;

            case -1:
                mixerEffectsSettings.amsGameOver.TransitionTo(mixerEffectsSettings.gameOverFadeTime);

                if (currentHeartbeatLoop != null)
                {
                    StopCoroutine(currentHeartbeatLoop);
                    currentHeartbeatLoop = StartCoroutine(HeartbeatLoop(heartbeatSettings.heartbeatDelGameOver));
                    _ = StartCoroutine(WaitAndStopHeartbeat(mixerEffectsSettings.gameOverFadeTime));
                }
                break;

            default:
                mixerEffectsSettings.amsMain.TransitionTo(mixerEffectsSettings.healthyFadeTime);
                if (currentHeartbeatLoop == null)
                {
                    break;
                }
                if (currentHeartbeatLoop != null)
                {
                    _ = StartCoroutine(WaitAndStopHeartbeat(mixerEffectsSettings.healthyFadeTime));
                }
                break;
        }
    }

    /// <summary>
    /// Gets the current lives from RunnerPlayer
    /// </summary>
    /// <returns>Amount of lives the player currently has</returns>
    private static int GetLives()
    {
        return RunnerPlayer.Lives;
    }

    /// <summary>
    /// A loop which plays a heartbeat and waits for the clip's length plus a delay time
    /// </summary>
    /// <param name="delay">Amount of time in seconds to delay after the heartbeat has finished playing</param>
    /// <returns></returns>
    private IEnumerator HeartbeatLoop(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return new WaitForSeconds(PlayHeartbeat().length);
        currentHeartbeatLoop = StartCoroutine(HeartbeatLoop(delay));
    }

    /// <summary>
    /// Waits for waitTime then stops the heartbeat loop
    /// Is used to stop the heartbeat after it has been faded out
    /// </summary>
    /// <param name="waitTime">Time to wait before stopping the currentHeartbeatLoop</param>
    /// <returns></returns>
    private IEnumerator WaitAndStopHeartbeat(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        StopCoroutine(currentHeartbeatLoop);
    }

    /// <summary>
    /// Plays the heartbeat from the audioWrapper specified in the heartbeatSettings
    /// </summary>
    /// <returns>The played clip</returns>
    private AudioClip PlayHeartbeat()
    {
        return PlayAudioClipWrapper(heartbeatSettings.heartbeatWrapper, heartbeatSettings.heartbeatSourceObj);
    }
}

