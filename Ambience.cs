using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Ambience : MonoBehaviour
{
    static public Ambience Current = null;

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

    public AudioMixerGroup output;

    [Serializable]
    public class AmbSound
    {
        [SerializeField]
        public AudioClip audioClip;
        [SerializeField, Range(-60f, 0f)]
        public float volDb = 0;
        [Range(-1200, 1200)]
        public float pitchCents = 0;
    }

    public List<AmbSound> ambSounds = new List<AmbSound>();

    void Start()
    {
        RunnerSounds.Current.StartAudioAmbience();
    }
}