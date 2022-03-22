using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AArmament : ScriptableObject
{
    [SerializeField]
    private string armamentName = string.Empty;

    [SerializeField]
    private IntPropertySO armamentLvl = null;

    [SerializeField]
    private GameObject armamentFirePrefab = null;

    //TODO: should these live else where to be easier to see all at once
    //in the inspector across all armaments? Armament settings?
    [SerializeField]
    private List<ArmamentRequirement> requirements = new List<ArmamentRequirement>();

    [SerializeField]
    private AAudioWrapperV2 utilizeAudio = null;

    public ArmamentRequirement GetRequirements()
    {
        foreach (ArmamentRequirement aq in requirements)
        {
            if (armamentLvl.Value == aq.ArmamentLvl)
            {
                return aq;
            }
        }

        Debug.LogError($"No armament requirement" +
            $"found for armament {armamentName} at lvl {armamentLvl}");
        return null;
    }

    public virtual void UseArmament(AudioWrapperSource audioSource, Transform spawnNode)
    {
        utilizeAudio?.PlayAudioWrapper(audioSource);
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
