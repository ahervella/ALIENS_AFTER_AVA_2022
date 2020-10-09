using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//aka, our class type of DJ
public class RunnerSounds : MonoBehaviour
{
    //DJs record players (only increases if needed for now)
    List<AudioSource> audioSources = new List<AudioSource>();

    //the single reference to the only DJ ever
    static public RunnerSounds Current
    {
        get
        {
            //if the DJ wasn't born, give birth
            //and they will be the "Current" DJ
            if (Current == null)
            {
                Current = new RunnerSounds();
            }

            //either way should have a living DJ by
            //the time we get here, so return them
            return Current;
        }

        //only within this class are we allowed to
        //say who the current DJ can be
        private set { Current = value; }
    }

    //any time anyone wants to play a sound from anywhere
    //use this method (with the current DJ)
    public void playSound(AudioClip audioClip, float vol = 1, float pitch = 1, float volVariation = 0, float pitchVariation = 0)
    {
        AudioSource CurrentSource = getAudioSource();

        CurrentSource.volume = vol + vol * (Random.Range(-volVariation, volVariation))/100;
        CurrentSource.pitch = pitch + pitch * (Random.Range(-pitchVariation, pitchVariation)) / 100;
        CurrentSource.clip = audioClip;
        CurrentSource.Play();
    }

    //get the first available record player
    private AudioSource getAudioSource()
    {
        AudioSource currSource;

        for (int i = 0; i < audioSources.Count; i++)
        {
            currSource = audioSources[i];
            if (!currSource.isPlaying)
            {
                return currSource;
            }
        }

        currSource = gameObject.AddComponent<AudioSource>();
        audioSources.Add(currSource);

        return currSource;
    }
}
