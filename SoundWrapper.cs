using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "soundWrapper", menuName = "ScriptableObjects/SoundWrapper", order = 1)]
public class SoundWrapper : ScriptableObject
{
    public GameObject gameObject;
    public List<AudioClip> audioClips = new List<AudioClip>();
    public float vol = 1;
    public float pitch = 1;
    public float volVariation = 0;
    public float pitchVariation = 0;
    //Audio oneshots do not require the creation of a new
    //audio source to play without interrupting
    //the currently playing sound, also cannot be stopped after starting
    public bool isOneShot = false;
    public bool isRandom = true;


    //gonna be in that library script:
    enum GameDataThing { PlayerHealth, GroundMat, Speed }; //etc.

    [SerializeField]
    private GameDataThing fuckingName
    {
        get
        {
            return fuckingName;
        }
        set
        {
            //get all of the DataShitToFeed that is used with this specific GameDataThing
            //and assign them to the listOfShitToFeedBasedOnDataThingWeChose
            fuckingName = value;
        }
    }


    [Serializable]
    private class DataShitToFeed
    {
        [SerializeField]
        public string ReadOnlyName { get; private set; }

        [SerializeField]
        public int ValueWeGiveToThisDataField { get; set; }
    }

    [SerializeField]
    private List<DataShitToFeed> listOfShitToFeedBasedOnDataThingWeChose;

    public List<AudioClipWrapper> audioClipWrappers = new List<AudioClipWrapper>();
}
