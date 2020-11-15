using System.Collections;
using UnityEngine;

//aka, our class type of DJ
public class RunnerSounds : MonoBehaviour
{

    //the single reference to the only DJ ever
    static public RunnerSounds Current = null;

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
    }

    public void PlayAudioWrapper(AAudioWrapper aw, GameObject soundObject)
    {
        aw.PlayAudioWrappers(soundObject);
    }

    //get the first available record player
    private AudioSource GetAudioSource(GameObject obj)
    {
        AudioSource[] audioSources = obj.GetComponents<AudioSource>();

        if (audioSources == null) //If there are no audio sources on the object
        {
            return obj.AddComponent<AudioSource>(); //Make one and use it
        }

        for (int i = 0; i < audioSources.Length; i++) //Checks for available audio sources
        {
            AudioSource thisSource = audioSources[i];

            if (!thisSource.isPlaying)//If it isn't playing
            {
                //Debug.Log("Sources used by " + obj + ": " + (i + 1));
                return thisSource; //Use this source
            }
        }
        Debug.Log("Object: '" + obj + "' required an additional audio source");
        return obj.AddComponent<AudioSource>();
    }

    public void PlayDelayed(AAudioWrapper aw, float del, GameObject soundObject)
    {
        StartCoroutine(DelayAndPlay(aw, del, soundObject));
    }

    IEnumerator DelayAndPlay(AAudioWrapper aw, float del, GameObject soundObject)
    {
        yield return new WaitForSeconds(del);
        aw.PlayAudioWrappers(soundObject);
    }

}

