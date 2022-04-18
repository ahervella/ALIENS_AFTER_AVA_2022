using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_ProjectileSettings", menuName = "ScriptableObjects/StaticData/SO_ProjectileSettings")]
public class SO_ProjectileSettings : ScriptableObject
{
    //NOTE: Since projectiles will often be used as prefab bases, will hold off on
    //making a settings for it for now

    /*
    [SerializeField]
    private WeaponEnum weaponType;
    public WeaponEnum WeaponType => weaponType;

    [SerializeField]
    private GameObject onImpactPrefab = null;
    public GameObject OnImpactPrefab => onImpactPrefab;

    [SerializeField]
    private float angleOffset = 0f;
    public float AngleOffset => angleOffset;

    [SerializeField]
    private Vector3 posOffset = default;
    public Vector3 PosOffset => posOffset;

    [SerializeField]
    private float speedPerSec = 10;
    public float SpeedPerSec => speedPerSec;

    [SerializeField]
    private bool destroyOnImpact = true;
    public bool DestroyOnImpact => destroyOnImpact;

    [SerializeField]
    private DestructionSprite destructionSpritePrefab = null;

    [SerializeField]
    private AAudioWrapperV2 travelAudio = null;

    [SerializeField]
    private AAudioWrapperV2 impactAudio = null;*/
}
