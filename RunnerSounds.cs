using System.Collections;
using System.Collections.Generic;
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
    /*
    static public GameObject Current
    {
        get
        {
            //if the DJ wasn't born, give birth
            //and they will be the "Current" DJ
            if (current == null)
            {
                current = this;//new RunnerSounds();
            }

            //either way should have a living DJ by
            //the time we get here, so return them
            return current;
        }

        //only within this class are we allowed to
        //say who the current DJ can be
        private set { current = value; }
    }
    */


    //any time anyone wants to play a sound from anywhere
    //use this method (with the current DJ)
    public void playSound(AudioClipWrapper acw, GameObject soundObject)
    {
        AudioSource CurrentSource = getAudioSource(soundObject, acw.isOneShot);

        CurrentSource.volume = acw.vol + acw.vol * (Random.Range(-acw.volVariation, acw.volVariation)) / 100;
        CurrentSource.pitch = acw.pitch + acw.pitch * (Random.Range(-acw.pitchVariation, acw.pitchVariation)) / 100;
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

        if (acw.isOneShot)
        {
            CurrentSource.PlayOneShot(clip);
        }
        else
        {
            CurrentSource.clip = clip;
            CurrentSource.Play();
        }
    }

    public void playSound(SoundWrapper sw, GameObject soundObject)
    {
        foreach (AudioClipWrapper acw in sw.audioClipWrappers)
        {
            playSound(acw, soundObject);
        }
    }

    //get the first available record player
    private AudioSource getAudioSource(GameObject obj, bool useOneShot)
    {
        AudioSource[] audioSources = obj.GetComponents<AudioSource>();
        AudioSource availSource = null;

        if (audioSources == null) //If there are no audio sources on the object
        {
            availSource = obj.AddComponent<AudioSource>(); //Make one and use it
        }
        else //If there are audiosources on the object
        {
            switch (useOneShot)
            {
                case true: //If it's a one-shot
                    availSource = audioSources[audioSources.Length - 1]; //Use the last source
                    //Debug.Log("OneShot source chosen");
                    break;
                case false:
                    for (int i = 0; i < audioSources.Length - 1; i++) //Checks for available audio sources
                    {
                        AudioSource thisSource = audioSources[i];

                        if (!thisSource.isPlaying)//If it isn't playing
                        {
                            availSource = thisSource; //Use this source
                        }
                        if (availSource != null) //If we chose an audio source
                        {
                            break; //Stop checking
                        }
                    }
                    break;
            }
        }
        return availSource;
    }
}

