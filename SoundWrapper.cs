﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundWrapper", order = 1)]
public class SoundWrapper : ScriptableObject
{
    public List<AudioClip> audioClips = new List<AudioClip>();
    public float vol = 1;
    public float pitch = 1;
    public float volVariation = 0;
    public float pitchVariation = 0;
    //Audio oneshots do not require the creation of a new
    //audio source to play without interrupting
    //the currently playing sound, also cannot be stopped after starting
    public bool isOneShot = true;
    public bool isRandom = true;
}
