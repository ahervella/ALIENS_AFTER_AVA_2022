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
        AudioSource CurrentSource = GetAudioSource(soundObject);

        CurrentSource.volume = Mathf.Pow(10, (acw.levelDb + Random.Range(-acw.volVrtnDb, acw.volVrtnDb)) / 20);
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

    public void StartAudioAmbience()
    {
        foreach (Ambience.AmbSound ambSound in Ambience.Current.ambSounds)
        {
            AudioSource CurrentSource = GetAudioSource(Ambience.Current.gameObject);
            CurrentSource.pitch = Mathf.Pow(2, ambSound.pitchCents / 1200);
            CurrentSource.loop = true;
            CurrentSource.clip = ambSound.audioClip;
            StartCoroutine(AudioFades.FadeIn(CurrentSource, 2, Mathf.Pow(10, ambSound.volDb / 20)));
        }
    }

    //get the first available record player
    private AudioSource GetAudioSource(GameObject obj)
    {
        AudioSource[] audioSources = obj.GetComponents<AudioSource>();

        if (audioSources == null) //If there are no audio sources on the object
        {
            Debug.Log("<<" + obj + ">> required an additional audio source");
            AudioSource newSource = obj.AddComponent<AudioSource>();
            newSource.outputAudioMixerGroup = obj.GetComponent<SourceProperties>().output;
            return newSource;
        }

        for (int i = 0; i < audioSources.Length; i++) //Checks for available audio sources
        {
            AudioSource thisSource = audioSources[i];

            if (!thisSource.isPlaying)//If it isn't playing
            {
                //Debug.Log("Source used by <<" + obj.name + ">>: " + (i + 1));
                return thisSource; //Use this source
            }
        }
        Debug.Log("<<" + obj + ">> required an additional audio source");
        AudioSource addedSource = obj.AddComponent<AudioSource>();
        addedSource.outputAudioMixerGroup = obj.GetComponent<SourceProperties>().output;
        return addedSource;
    }

    public void PlayDelayed(AAudioWrapper aw, float del, GameObject soundObject)
    {
        Coroutine newCR = StartCoroutine(DelayAndPlay(aw, del, soundObject));

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
        yield return new WaitForSeconds(del);
        aw.PlayAudioWrappers(soundObject);
    }

    public void StopAllDelayedSounds(GameObject go)
    {
        if (soundCRs.TryGetValue(go, out var crList))
        {
            foreach(Coroutine cr in crList)
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

