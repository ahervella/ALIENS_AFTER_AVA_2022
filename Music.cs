using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    static public Music Current = null;

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

    [Serializable]
    public class MusicLoop
    {
        [SerializeField]
        public AudioClip audioClip;
        [SerializeField, Range(-60f, 0f)]
        public float volDb = 0;
    }

    public List<MusicLoop> musicLoops = new List<MusicLoop>();

    void Start()
    {
        RunnerSounds.Current.StartMusic();
    }
}
