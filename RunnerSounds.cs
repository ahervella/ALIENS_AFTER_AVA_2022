using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//aka, our class type of DJ
public class RunnerSounds : MonoBehaviour
{
    //DJs record players (only increases if needed for now)
    List<AudioSource> audioSources = new List<AudioSource>();

    
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
    public void playSound(SoundWrapper sw, GameObject soundObject)
    {
        AudioSource CurrentSource = getAudioSource(soundObject, sw.isOneShot);

        CurrentSource.volume = sw.vol + sw.vol * (Random.Range(-sw.volVariation, sw.volVariation))/100;
        CurrentSource.pitch = sw.pitch + sw.pitch * (Random.Range(-sw.pitchVariation, sw.pitchVariation)) / 100;
        if (sw.isRandom)
        {
            CurrentSource.clip = sw.audioClips[Random.Range(0, sw.audioClips.Count - 1)];
            sw.audioClips.Remove(CurrentSource.clip);
            sw.audioClips.Add(CurrentSource.clip);
        }
        else
        {
            // :/
            Debug.Log(sw.name + " IS NOT RANDOM >>:(");
        }
        
        CurrentSource.Play();

    }

    //get the first available record player
    private AudioSource getAudioSource(GameObject obj, bool useOneShot)
    {
        AudioSource currSource;
        /*
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
        */

        currSource = obj.GetComponent<AudioSource>();

        if (currSource == null || (!useOneShot && currSource.isPlaying))
        {
            currSource = obj.AddComponent<AudioSource>();
        }

        return currSource;
        
    }
}

