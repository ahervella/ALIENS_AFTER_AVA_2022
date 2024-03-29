﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//TODO: rename to SO_AArmament
public abstract class AArmament : ScriptableObject
{
    [SerializeField]
    private string armamentName = string.Empty;

    [SerializeField]
    private IntPropertySO armamentLvl = null;

    [SerializeField]
    private PSO_FillBarQuant armamentCoolDownPSO = null;
    public PSO_FillBarQuant ArmamentCoolDownPSO => armamentCoolDownPSO;

    [SerializeField]
    private IntPropertySO energyReqPSO = null;

    [SerializeField]
    private GameObject armamentFirePrefab = null;

    [SerializeField]
    private GameObject armamentHUDIconPrefab = null;
    public GameObject ArmamentHUDIconPrefab => armamentHUDIconPrefab;

    [SerializeField]
    private List<PlayerActionEnum> applicableActions = new List<PlayerActionEnum>();
    public List<PlayerActionEnum> ApplicableActions => applicableActions;

    //TODO: should these live else where to be easier to see all at once
    //in the inspector across all armaments? Armament settings?
    [SerializeField]
    private List<ArmamentRequirement> requirements = new List<ArmamentRequirement>();

    [SerializeField]
    private bool oneInstanceAtATime = true;

    [SerializeField]
    private BoolPropertySO optionalArmamentActivePSO = null;

    [NonSerialized]
    private ArmamentRequirement cachedReq = null;

    public bool CanSpawnAnotherInstance =>
    !oneInstanceAtATime || !(optionalArmamentActivePSO?.Value ?? false);

    public void CacheArmamentLevelRequirements()
    {
        GetRequirements();
    }

    public ArmamentRequirement GetRequirements()
    {
        if (cachedReq != null) { return cachedReq; }

        foreach (ArmamentRequirement aq in requirements)
        {
            if (armamentLvl.Value == aq.ArmamentLvl)
            {
                energyReqPSO.ModifyValueNoInvoke(-energyReqPSO.Value + aq.EnergyBlocks);
                return aq;
            } 
        }

        Debug.LogError($"No armament requirement" +
            $"found for armament {armamentName} at lvl {armamentLvl}");
        return null;
    }

    public virtual void UseArmament(Transform spawnNode)
    {
        optionalArmamentActivePSO?.ModifyValue(true);
        Instantiate(armamentFirePrefab, spawnNode);
    }
}

[Serializable]
public class ArmamentRequirement
{
    [SerializeField]
    private int armamentLvl = -1;
    public int ArmamentLvl => armamentLvl;

    [SerializeField]
    private int energyBlocks = 1;
    public int EnergyBlocks => energyBlocks;

    [SerializeField]
    private float coolDownTime = 0;
    public float CoolDownTime => coolDownTime;
}
