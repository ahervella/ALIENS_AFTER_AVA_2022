using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioWrapperSource))]
public class TerrHazard : TerrAddon
{
    [SerializeField]
    private PlayerActionEnum requiredAvoidAction = PlayerActionEnum.NONE;

    //The percent in each dimension we want to actually have on the edge tiles
    //of this hazard (if it's 3x1, and this is (0.8, 0.4, 0.7), then the dims of
    //the hitBox are (2.8, 0.4, 0.7) * (tileWidth, floorHeight, tileHeight)
    [SerializeField]
    private Vector3PropertySO hitBoxDimEdgePercents = null;

    [SerializeField]
    protected SO_TerrSettings terrSettings = null;

    [SerializeField]
    private BoxColliderSP hitBox = null;

    [SerializeField]
    private BoxColliderSP energyRewardBox = null;

    [SerializeField]
    private AAudioWrapperV2 impactAudio = null;

    protected AudioWrapperSource audioSource;

    //for aliens that inherit have their own serialized field
    protected PlayerActionEnum hazardTakeDownReqAction = PlayerActionEnum.NONE;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioWrapperSource>();
        InitBoxColliders();


    }

    private void InitBoxColliders()
    {
        SetHitBoxDimensions();
        SetRewardBoxDimensions(hitBox.Box());
        hitBox.SetOnTriggerEnterMethod(OnTriggerEnterDamageHitBox);
        energyRewardBox.SetOnTriggerExitMethod(OnTriggerExitRewardBox);
    }

    private void SetHitBoxDimensions()
    {
        Vector3 hitBoxDimensions = new Vector3(
            Dimensions().x * terrSettings.TileDims.x,
            terrSettings.FloorHeight,
            Dimensions().y * terrSettings.TileDims.y);

        hitBoxDimensions -= new Vector3(
            terrSettings.TileDims.x * (1 - hitBoxDimEdgePercents.Value.x),
            terrSettings.FloorHeight * (1 - hitBoxDimEdgePercents.Value.y),
            terrSettings.TileDims.y * (1 - hitBoxDimEdgePercents.Value.z));

        hitBox.Box().size = hitBoxDimensions;
        hitBox.Box().center = new Vector3(0, hitBoxDimensions.y / 2f, 0);
    }

    private void SetRewardBoxDimensions(BoxCollider hitBox)
    {
        //Hack: save computation by using hit box values
        Vector3 rewardBoxDims = hitBox.size + new Vector3(0, 0, terrSettings.RewardBoxLengthFront);

        energyRewardBox.Box().size = rewardBoxDims;
        energyRewardBox.Box().center = new Vector3(0, rewardBoxDims.y / 2f, -(rewardBoxDims.z - hitBox.size.z) / 2f);
    }

    private void OnTriggerEnterDamageHitBox(Collider other)
    {
        PlayerRunner player = other.gameObject.GetComponent<PlayerRunner>();
        if (player != null)
        {
            impactAudio.PlayAudioWrapper(audioSource);
            player.OnEnterHazard(requiredAvoidAction, hazardTakeDownReqAction, TerrAddonEnum, out _);
            return;
        }

        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.OnEnteredHazard(this);
        }
    }


    protected virtual void OnTriggerExitRewardBox(Collider other)
    {
        PlayerRunner player = other.gameObject.GetComponent<PlayerRunner>();
        if (player != null)
        {
            player.OnExitHazardRewardArea();
        }
    }
}
