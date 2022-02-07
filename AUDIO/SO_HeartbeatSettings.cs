using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SO_HeartbeatSettings", menuName = "ScriptableObjects/Audio/SO_HeartbeatSettings")]
public class SO_HeartbeatSettings : SO_LoopAudioSettings
{
    [SerializeField]
    private List<HeartbeatDelayWrapper> heartbeatDelays = new List<HeartbeatDelayWrapper>();
    public List<HeartbeatDelayWrapper> HeartbeatDelays => heartbeatDelays;
}

[Serializable]
public class HeartbeatDelayWrapper
{
    [SerializeField]
    private int livesLeft;
    public int LivesLeft => livesLeft;

    [SerializeField]
    [Range(0f, 10f)]
    private float heartBeatDelay = 1;
    public float HeartBeatDelay => heartBeatDelay;
}
