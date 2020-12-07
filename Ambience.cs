﻿using System;
using System.Collections.Generic;
using UnityEngine;

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

    [Serializable]
    public class AmbLoop
    {
        [SerializeField]
        public AudioClip audioClip;
        [SerializeField, Range(-60f, 0f)]
        public float volDb = 0;
        [Range(-1200, 1200)]
        public float pitchCents = 0;
    }

    public List<AmbLoop> ambLoops = new List<AmbLoop>();

    void Start()
    {
        RunnerSounds.Current.StartAmbience();
    }
}