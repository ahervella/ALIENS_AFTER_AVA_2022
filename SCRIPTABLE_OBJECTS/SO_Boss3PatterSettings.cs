using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SO_Boss3PatterSettings", menuName = "ScriptableObjects/StaticData/SO_Boss3PatterSettings")]
public class SO_Boss3PatterSettings : ScriptableObject
{
    [SerializeField]
    private List<PatterGroupWrapper> groups = new List<PatterGroupWrapper>();

    public SO_Boss3PatternWrapper GetRandPatternWrapper(ZonePhaseEnum zonePhase)
    {
        PatterGroupWrapper pgw = GetPG(zonePhase);
        return pgw.Patterns[Random.Range(0, pgw.Patterns.Count)];
    }
    public float GetIdleTime(ZonePhaseEnum zonePhase)
    {
        return GetPG(zonePhase).IdleTime;
    }

    private PatterGroupWrapper GetPG(ZonePhaseEnum zonePhase)
    {
        return GetWrapperFromFunc(groups, pgw => pgw.ZonePhase, zonePhase, LogEnum.ERROR, null);
    }

    [Serializable]
    private class PatterGroupWrapper
    {
        [SerializeField]
        private ZonePhaseEnum zonePhase = default;
        public ZonePhaseEnum ZonePhase => zonePhase;

        [SerializeField]
        private float idleTime = default;
        public float IdleTime => idleTime;

        [SerializeField]
        private List<SO_Boss3PatternWrapper> patterns = new List<SO_Boss3PatternWrapper>();
        public List<SO_Boss3PatternWrapper> Patterns => patterns;
    }
}


