using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "SO_PlayerRunnerSettings", menuName = "ScriptableObjects/StaticData/SO_PlayerRunnerSettings")]
public class SO_PlayerRunnerSettings : ScriptableObject
{
    //TODO: evaluate once working runner level if we want to break these up
    [SerializeField]
    private float lifeRecoveryTime;
    public float LifeRecoveryTime => lifeRecoveryTime;

    [SerializeField]
    private float startingEnergyBlocks;
    public float StartingEnergyBlocks => startingEnergyBlocks;

    [SerializeField]
    private float laneChangeTime;
    public float LaneChangeTime => laneChangeTime;

    [SerializeField]
    private float laneChangeDelay;
    public float LaneChangeDelay => laneChangeDelay;

    [SerializeField]
    private float startRowsFromEnd = 1;
    public float StartRowsFromEnd => startRowsFromEnd;

    [SerializeField]
    private List<SO_CameraAngle> actionCameraAngles = new List<SO_CameraAngle>();
    public SO_CameraAngle GetActionCameraAngle(PlayerActionEnum action) => actionCameraAngles.First(ca => action == ca.ActionTag);
}
