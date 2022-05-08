using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

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
    public BoxColliderSP HitBox => hitBox;

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
        SetHitBoxDimensions(hitBox, Dimensions(), terrSettings, hitBoxDimEdgePercents);
        SetRewardBoxDimensions(hitBox.Box());
        hitBox.SetOnTriggerEnterMethod(OnTriggerEnterDamageHitBox);
        energyRewardBox.SetOnTriggerExitMethod(OnTriggerExitRewardBox);
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
            player.OnEnterHazard(requiredAvoidAction, hazardTakeDownReqAction, TerrAddonEnum, out bool dodged);
            if (!dodged)
            {
                impactAudio.PlayAudioWrapper(audioSource);
            }
            return;
        }

        //TODO use a recursive search in case we structure prefabs differently
        //(also change for boss script)
        Projectile projectile = other.transform.parent.gameObject.GetComponent<Projectile>();
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
