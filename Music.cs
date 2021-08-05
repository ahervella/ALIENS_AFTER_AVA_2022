using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : Singleton<Music>
{
    protected override Music GetSelf() => this;

    [Serializable]
    public class MusicLoop
    {
        [SerializeField]
        public AudioClip audioClip;
        [SerializeField, Range(-60f, 0f)]
        public float volDb = 0;
    }

    public List<MusicLoop> musicLoops = new List<MusicLoop>();

    /*
    void Start()
    {
        RunnerSounds.Current.StartMusic();
    }*/
}
