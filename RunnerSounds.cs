using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//aka, our class type of DJ
public class RunnerSounds : MonoBehaviour
{

    //the single reference to the only DJ ever
    static public RunnerSounds Current = null;

    private Dictionary<GameObject, List<Coroutine>> soundCRs = new Dictionary<GameObject, List<Coroutine>>();

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

    //any time anyone wants to play a sound from anywhere
    //use this method (with the current DJ)
    public void PlayAudioClip(AudioClipWrapper acw, GameObject soundObject)
    {
        AudioSource CurrentSource = TryGetAudioSource(soundObject);
        if (CurrentSource == null)
        {
            return;
        }
        CurrentSource.volume = Mathf.Pow(10, (acw.LevelDb + Random.Range(-acw.volVrtnDb, acw.volVrtnDb)) / 20);
        CurrentSource.pitch = Mathf.Pow(2, (acw.pitchCents + Random.Range(-acw.pitchVrtnCents, acw.pitchVrtnCents)) / 1200);
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

        CurrentSource.clip = clip;
        CurrentSource.Play();
        //Debug.Log("Clip <<" + clip.name + ">> played from <<" + soundObject.name +">>");
    }

    public void PlayAudioWrapper(AAudioWrapper aw, GameObject soundObject)
    {
        //TODO: do we want this here automatically, or make it to only do so on a bool flag?
        StopAllDelayedSounds(soundObject);
        aw.PlayAudioWrappers(soundObject);
    }

    public void StartAmbience()
    {
        Debug.Log(Ambience.Current.ambLoops.Count);
        foreach (Ambience.AmbLoop loop in Ambience.Current.ambLoops)
        {
            AudioSource CurrentSource = TryGetAudioSource(Ambience.Current.gameObject);
            if (CurrentSource == null)
            {
                return;
            }
            CurrentSource.pitch = Mathf.Pow(2, loop.pitchCents / 1200);
            CurrentSource.loop = true;
            CurrentSource.clip = loop.audioClip;
            StartCoroutine(AudioFades.FadeIn(CurrentSource, 1, Mathf.Pow(10, loop.volDb / 20)));
        }
    }

    public void StartMusic()
    {
        Debug.Log(Music.Current.musicLoops.Count);
        foreach (Music.MusicLoop loop in Music.Current.musicLoops)
        {
            AudioSource CurrentSource = TryGetAudioSource(Music.Current.gameObject);
            if (CurrentSource == null)
            {
                return;
            }
            CurrentSource.loop = true;
            CurrentSource.clip = loop.audioClip;
            StartCoroutine(AudioFades.FadeIn(CurrentSource, 1, Mathf.Pow(10, loop.volDb / 20)));
        }
    }

    /// <summary>
    /// Trys to get the first available AudioSource.
    /// Returns 
    /// </summary>
    /// <param name="obj">GameObject from which to play sound</param>
    /// <returns>The audio source to be used.
    /// Else returns null if object needs new AudioSource
    /// and does not have a SourceProperties component</returns>
    private AudioSource TryGetAudioSource(GameObject obj)
    {
        AudioSource[] audioSources = obj.GetComponents<AudioSource>();

        if (audioSources != null)
        {
            for (int i = 0; i < audioSources.Length; i++) //Checks for available audio sources
            {
                AudioSource thisSource = audioSources[i];

                if (!thisSource.isPlaying)//If it isn't playing
                {
                    //Debug.Log("Source used by <<" + obj.name + ">>: " + (i + 1));
                    return thisSource; //Use this source
                }
            }
        }

        SourceProperties sourceProperties = obj.GetComponent<SourceProperties>();

        if (sourceProperties == null)
        {
            Debug.LogError("No SourceProperties Component on the gameobject: " + obj.name);
            return null;
        }

        Debug.Log("<<" + obj + ">> required an additional audio source");
        AudioSource addedSource = obj.AddComponent<AudioSource>();
        AssignSourceProperties(addedSource, sourceProperties);
        return addedSource;
    }

    public void AssignSourceProperties(AudioSource source, SourceProperties sp)
    {
        source.outputAudioMixerGroup = sp.output;
        source.spatialBlend = sp.spatialBlend;
        source.maxDistance = sp.maxDist;
        source.rolloffMode = AudioRolloffMode.Custom;
    }
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

    IEnumerator DelayAndPlay(AAudioWrapper aw, float del, GameObject soundObject)
    {
        yield return new WaitForSecondsRealtime(del);
        if (soundObject != null)
        {
            aw.PlayAudioWrappers(soundObject);
        }
    }

    public void StopAllDelayedSounds(GameObject go)
    {
        if (soundCRs.TryGetValue(go, out var crList))
        {
            foreach (Coroutine cr in crList)
            {
                StopCoroutine(cr);
            }
            //TODO: is this expensive to add and remove so frequently? Cause
            //we don't want to have deleted object keys chilling in there
            //as long as a player hasn't lost a run...
            soundCRs.Remove(go);
        }
    }

}

