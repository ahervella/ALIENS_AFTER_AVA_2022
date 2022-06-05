using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using System;

[RequireComponent(typeof(AudioWrapperSource))]
public class TerrHazard : TerrAddon
{
    //TODO: should probably change this name to conicide with the option of not using
    //custom hitboxes but don't want to mess with serialization right now
    //[SerializeField]
    //private PlayerActionEnum requiredAvoidAction = PlayerActionEnum.NONE;

    //public PlayerActionEnum GetRequiredAvoidAction(BoxColliderSP hb)
    //{
    //    return HelperUtil.GetRequiredAvoidAction(hitBox, reqActionDict, hb);
    //}

    //The percent in each dimension we want to actually have on the edge tiles
    //of this hazard (if it's 3x1, and this is (0.8, 0.4, 0.7), then the dims of
    //the hitBox are (2.8, 0.4, 0.7) * (tileWidth, floorHeight, tileHeight)
    [SerializeField]
    private Vector3PropertySO hitBoxDimEdgePercents = null;

    //[SerializeField]
    //private int hitBoxHeight = 1;

    [SerializeField]
    protected SO_TerrSettings terrSettings = null;

    [SerializeField]
    private SO_LayerSettings layerSettings = null;

    [SerializeField]
    private BoxColliderSP hitBox = null;

    //[SerializeField]
    //private bool useCustomHitBoxes = false;

    [SerializeField]
    private List<HitBoxWrapper> customHitBoxes = new List<HitBoxWrapper>();

    [SerializeField]
    private BoxColliderSP energyRewardBox = null;

    [SerializeField]
    private AAudioWrapperV2 impactAudio = null;

    [SerializeField]
    protected bool tussleOnAttack = false;

    protected AudioWrapperSource audioSource;

    //for aliens that inherit have their own serialized field
    protected PlayerActionEnum hazardTakeDownReqAction = PlayerActionEnum.NONE;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioWrapperSource>();
        InitBoxColliders();
    }

    //TODO: add some sort of verification to make sure that for hazards
    //with only one hit box that we extend across the whole width, avoid
    //human error?
    private void InitBoxColliders()
    {
        MakeCustomHitBoxes(
                hitBox,
                Dimensions(),
                terrSettings,
                hitBoxDimEdgePercents,
                customHitBoxes);

        foreach (HitBoxWrapper hbw in customHitBoxes)
        {
            ApplyHitBoxSizeErrorFix(hbw.InstancedHB, towardsOrAwayFromPlayer: true);
            hbw.InstancedHB.SetOnTriggerEnterMethod(
                coll => OnTriggerEnterDamageHitBox(coll, hbw.InstancedHB));
        }

        //if its only one hit box, can save time and use the one hit box for the reward hitbox
        //assuming it should take up the full specified dimensions
        if (customHitBoxes.Count == 1)
        {
            SetRewardBoxDimensionsFromHB(
                customHitBoxes[0].InstancedHB.Box(), energyRewardBox, terrSettings);
            
        }
        else
        {
            SetRewardBoxDimensions(
                hitBox,
                energyRewardBox,
                Dimensions(),
                //TODO: until we make reward boxes  match
                //custom hit boxes, stick with this
                1,
                terrSettings,
                hitBoxDimEdgePercents);
        }

        //hitBox.SetOnTriggerEnterMethod(OnTriggerEnterDamageHitBox);
        energyRewardBox.SetOnTriggerExitMethod(OnTriggerExitRewardBox);
    }

    //private void MakeCustomHitBoxes()
    //{
    //    foreach(HitBoxWrapper hbw in customHitBoxes)
    //    {
    //        BoxColliderSP instance = Instantiate(hitBox, hitBox.transform.parent);
    //        int width = hbw.MaxXRange - hbw.MinXRange;
    //        Vector2Int dims = new Vector2Int(width, Dimensions().y);

    //        SetHitBoxDimensions(instance, dims, hitBoxHeight, terrSettings, hitBoxDimEdgePercents);

    //        float centerOffset = (width - Dimensions().x) / 2f + hbw.MinXRange;
    //        centerOffset *= terrSettings.TileDims.x;
    //        instance.Box().center = new Vector3(centerOffset, instance.Box().center.y, instance.Box().center.z);

    //        reqActionDict.Add(instance, hbw.CustomAvoidAction);
    //    }

    //    //disable original so we don't overlap with that
    //    hitBox.enabled = false;
    //}

    //private void SetRewardBoxDimensionsFromHB(BoxCollider hitBox)
    //{
    //    //Hack: save computation by using hit box values
    //    Vector3 rewardBoxDims = hitBox.size + new Vector3(0, 0, terrSettings.RewardBoxLengthFront);

    //    energyRewardBox.Box().size = rewardBoxDims;
    //    energyRewardBox.Box().center = new Vector3(0, rewardBoxDims.y / 2f, -(rewardBoxDims.z - hitBox.size.z) / 2f);
    //}

    private void OnTriggerEnterDamageHitBox(Collider other, BoxColliderSP hazardHb)
    {
        BoxColliderSP hb = other.gameObject.GetComponent<BoxColliderSP>();
        PlayerRunner player = hb?.RootParent?.GetComponent<PlayerRunner>();

        if (player != null)
        {
            player.OnEnterHazard(
                hazardHb.RequiredAvoidAction,
                hazardTakeDownReqAction,
                TerrAddonEnum,
                tussleOnAttack,
                out bool dodged,
                out bool destroySelf);
            if (destroySelf)
            {
                SafeDestroy(gameObject);
            }
            if (!dodged)
            {
                //TODO: custom impact sounds per hit box wrapper
                impactAudio.PlayAudioWrapper(audioSource);
            }
            return;
        }

        //TODO use a recursive search in case we structure prefabs differently
        //(also change for boss script)
        Projectile projectile = other.transform.parent.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.OnEnteredHazard(this, hazardHb);
        }
    }


    protected virtual void OnTriggerExitRewardBox(Collider other)
    {
        BoxColliderSP hb = other.gameObject.GetComponent<BoxColliderSP>();
        PlayerRunner player = hb?.RootParent.GetComponent<PlayerRunner>();

        if (player != null)
        {
            player.OnExitHazardRewardArea();
        }
    }
}
