using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Boss3AnimationSettings", menuName = "ScriptableObjects/StaticData/SO_Boss3AnimationSettings")]
public class SO_Boss3AnimationSettings : SO_AnimationSettings<Boss3State>
{
    //TODO: share via a new class with boss 1
    [SerializeField]
    private float yOscillationTileAmnt = 10f;
    public float YOscillationTileAmnt => yOscillationTileAmnt;

    [SerializeField]
    private float yOscillationTime = 3f;
    public float YOscillationTime => yOscillationTime;

    [SerializeField]
    private float xOscillationTileAmnt = 5f;
    public float XOscillationTileAmnt => xOscillationTileAmnt;

    [SerializeField]
    private float xOscillationTime = 3f;
    public float XOscillationTime => xOscillationTime;
}
