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
    [SerializeField]
    private PlayerActionEnum requiredAvoidAction = PlayerActionEnum.NONE;

    public PlayerActionEnum GetRequiredAvoidAction(BoxColliderSP hb)
    {
        if (!reqActionDict.ContainsKey(hb))
        {
            Debug.LogError("Should always be in the dictionary here!");
            return PlayerActionEnum.NONE;
        }

        return reqActionDict[hb];
    }

    //The percent in each dimension we want to actually have on the edge tiles
    //of this hazard (if it's 3x1, and this is (0.8, 0.4, 0.7), then the dims of
    //the hitBox are (2.8, 0.4, 0.7) * (tileWidth, floorHeight, tileHeight)
    [SerializeField]
    private Vector3PropertySO hitBoxDimEdgePercents = null;

    [SerializeField]
    protected SO_TerrSettings terrSettings = null;

    [SerializeField]
    private SO_LayerSettings layerSettings = null;

    [SerializeField]
    private BoxColliderSP hitBox = null;
    public BoxColliderSP HitBox => hitBox;

    [SerializeField]
    private bool useCustomHitBoxes = false;

    [SerializeField]
    private List<HitBoxWrapper> customHitBoxes = new List<HitBoxWrapper>();

    [Serializable]
    private class HitBoxWrapper
    {
        [SerializeField]
        private int minXRange = 0;
        public int MinXRange => minXRange;

        [SerializeField]
        private int maxXRange = 1;
        public int MaxXRange => maxXRange;

        [SerializeField]
        private PlayerActionEnum customAvoidAction = PlayerActionEnum.NONE;
        public PlayerActionEnum CustomAvoidAction => customAvoidAction;
    }

    private Dictionary<BoxColliderSP, PlayerActionEnum> reqActionDict = new Dictionary<BoxColliderSP, PlayerActionEnum>();

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

    private void InitBoxColliders()
    {
        //TODO: currently only setting the default hit box no matter if we doing custom
        //hit boxes so that the reward hit box can do it's thing, fix so we only
        //do it for non custom hit box setups
        SetHitBoxDimensions(hitBox, Dimensions(), terrSettings, hitBoxDimEdgePercents);
        SetRewardBoxDimensions(hitBox.Box());

        reqActionDict.Clear();
        if (useCustomHitBoxes)
        {
            MakeCustomHitBoxes();
        }
        else
        {
            reqActionDict.Add(hitBox, requiredAvoidAction);
        }

        
        foreach(KeyValuePair<BoxColliderSP, PlayerActionEnum> kvp in reqActionDict)
        {
            BoxCollider box = kvp.Key.Box();
            PlayerActionEnum reqAction = kvp.Value;

            //TODO: realized that I set the standard of tiles and hit box length
            //with half of it behind the terr object. Cleaner way to fix?
            //Originally had to do this because the grapple was colliding with the back of the hazard
            box.size = new Vector3(box.size.x, box.size.y, box.size.z / 2f);
            box.center = new Vector3(box.center.x, box.center.y, -box.size.z / 2f);

            kvp.Key.SetOnTriggerEnterMethod(coll => OnTriggerEnterDamageHitBox(coll, kvp.Key));
        }

        //hitBox.SetOnTriggerEnterMethod(OnTriggerEnterDamageHitBox);
        energyRewardBox.SetOnTriggerExitMethod(OnTriggerExitRewardBox);
    }

    private void MakeCustomHitBoxes()
    {
        foreach(HitBoxWrapper hbw in customHitBoxes)
        {
            BoxColliderSP instance = Instantiate(hitBox, hitBox.transform.parent);
            int width = hbw.MaxXRange - hbw.MinXRange;
            Vector2Int dims = new Vector2Int(width, Dimensions().y);

            SetHitBoxDimensions(instance, dims, terrSettings, hitBoxDimEdgePercents);

            float centerOffset = (width - Dimensions().x) / 2f + hbw.MinXRange;
            centerOffset *= terrSettings.TileDims.x;
            instance.Box().center = new Vector3(centerOffset, instance.Box().center.y, instance.Box().center.z);

            reqActionDict.Add(instance, hbw.CustomAvoidAction);
        }

        //disable original so we don't overlap with that
        hitBox.enabled = false;
    }

    private void SetRewardBoxDimensions(BoxCollider hitBox)
    {
        //Hack: save computation by using hit box values
        Vector3 rewardBoxDims = hitBox.size + new Vector3(0, 0, terrSettings.RewardBoxLengthFront);

        energyRewardBox.Box().size = rewardBoxDims;
        energyRewardBox.Box().center = new Vector3(0, rewardBoxDims.y / 2f, -(rewardBoxDims.z - hitBox.size.z) / 2f);
    }

    private void OnTriggerEnterDamageHitBox(Collider other, BoxColliderSP hitBox)
    {
        PlayerRunner player = other.gameObject.GetComponent<PlayerRunner>();
        if (player != null)
        {
            player.OnEnterHazard(
                reqActionDict[hitBox],
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
                impactAudio.PlayAudioWrapper(audioSource);
            }
            return;
        }

        //TODO use a recursive search in case we structure prefabs differently
        //(also change for boss script)
        Projectile projectile = other.transform.parent.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.OnEnteredHazard(this, hitBox);
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
