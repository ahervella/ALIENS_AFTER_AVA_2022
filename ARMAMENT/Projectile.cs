using System.Collections;
using System.Collections.Generic;
using PowerTools;
using UnityEngine;
using static HelperUtil;

[RequireComponent(typeof(SafeAudioWrapperSource))]
public class Projectile : MovingNode
{
    [SerializeField]
    private Vector3PropertySO hitBoxDimEdgePerc = null;

    [SerializeField]
    private PSO_TerrainTreadmillNodes terrTreadmillNodesPSO = null;

    [SerializeField]
    private Vector2Int widthHeightDims = new Vector2Int(1, 1);

    [SerializeField]
    private TREADMILL_ATTACHMENT treadmillAttachment = TREADMILL_ATTACHMENT.NONE;

    private enum TREADMILL_ATTACHMENT { NONE = 0, HORIZONTAL = 1, HORIZ_VERT = 2 }

    [SerializeField]
    private BoxColliderSP hitBox = null;

    [SerializeField]
    private SpriteAnim spriteAnim = null;

    [SerializeField]
    private WeaponEnum weaponType;

    [SerializeField]
    private PlayerActionEnum requiredAvoidAction = PlayerActionEnum.NONE;

    [SerializeField]
    private float spriteYPosPercFloorHeight = 0.5f;

    [SerializeField]
    private GameObject OnImpactPrefab = null;

    [SerializeField]
    private Vector3 posOffset = default;

    [SerializeField]
    private bool autoAlignToNearestLane = true;

    [SerializeField]
    private float timeSprite2Align2NearestLane = 0.5f;

    [SerializeField]
    private bool destroyOnImpact = true;

    [SerializeField]
    private DestructionSprite destructionSpritePrefab = null;

    [SerializeField]
    private AAudioWrapperV2 travelAudio = null;

    [SerializeField]
    private AAudioWrapperV2 impactAudio = null;

    [SerializeField]
    private bool isAlienProjectile = false;

    [SerializeField]
    private bool destroyTerrHazards = true;

    [SerializeField]
    private PSO_CurrentPlayerAction currPlayerAction = null;

    [SerializeField]
    private SO_DamageQuantSettings damageSettings = null;

    private bool highOrLowShot =>
        !isAlienProjectile && currPlayerAction.Value == PlayerActionEnum.JUMP;

    private AudioWrapperSource audioSource;

    private Transform mzTrans;

    private BoxColliderSP sourceHitBox = null;

    protected override void OnAwake()
    {
        slope *= isAlienProjectile ? -1 : 1;

        audioSource = GetComponent<AudioWrapperSource>();
        travelAudio?.PlayAudioWrapper(audioSource);

        SetMuzzleFlashTranform(transform);
    }

    public void SetFireSourceHitBox(BoxColliderSP sourceHitBox)
    {
        this.sourceHitBox = sourceHitBox;
    }

    public void SetMuzzleFlashTranform(Transform mzTrans)
    {
        this.mzTrans = mzTrans;
    }

    //In Start so we don't have these positions overwritten if we
    //had use HelperUtil.InstantiateAndSetPosition
    private void Start()
    {
        ConfigHitBox();
        SetSpawnPosition();
    }

    private void ConfigHitBox()
    {
        //TODO: is this too jank or can I just resort to using on collision or on trigger and return
        //depending on the isAlienProjectile flag?
        hitBox.Box().isTrigger = true;//!isAlienProjectile;
        SetHitBoxDimensionsAndPos(
            hitBox.Box(),
            new Vector2(widthHeightDims.x, 1),
            widthHeightDims.y,
            terrSettings,
            hitBoxDimEdgePerc);


        hitBox.SetOnTriggerEnterMethod(OnTriggerEnter);
    }

    private void SetSpawnPosition()
    {
        if (treadmillAttachment == TREADMILL_ATTACHMENT.HORIZONTAL)
        {
            terrTreadmillNodesPSO.Value.AttachTransform(transform, horizOrVert: true);
        }
        else if (treadmillAttachment == TREADMILL_ATTACHMENT.HORIZ_VERT)
        {
            terrTreadmillNodesPSO.Value.AttachTransform(transform, horizOrVert: false);
        }

        float xPos = autoAlignToNearestLane ?
            GetLaneXPosition(GetLaneIndexFromPosition(transform.position.x, terrSettings), terrSettings)
            : transform.position.x;

        float yPos = highOrLowShot ? terrSettings.FloorHeight : 0;//0;// GetFloorYPosition(FloorIndex, terrSettings);



        transform.position = new Vector3(xPos, yPos, transform.position.z);

        Vector3 originalSpriteLocalPos = mzTrans.position - transform.position;

        float spriteFinalYLocalPos = terrSettings.FloorHeight * spriteYPosPercFloorHeight + yPos;
        Vector3 finalLocalPos = new Vector3(0, spriteFinalYLocalPos, 0);

        StartCoroutine(SpritePosTween(originalSpriteLocalPos, finalLocalPos));

        if (!autoAlignToNearestLane)
        {
            transform.position += posOffset;
        }
    }

    private IEnumerator SpritePosTween(Vector3 originalSpriteLocalPos, Vector3 finalLocalPos)
    {
        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime;
            Vector3 currLocalPos = Vector3.Lerp(originalSpriteLocalPos, finalLocalPos, EasedPercent(perc));
            spriteAnim.transform.localPosition = currLocalPos;
            yield return null;
        }
    }

    public void OnEnteredHazard(TerrHazard hazard, BoxColliderSP hitBox)
    {
        //Null check on the sourceHitBox for bullets instanced from player
        if (hitBox.RootParent == sourceHitBox?.RootParent) { return; }

        //TODO: handle aliens being stunned by grapple differently in
        if (hazard is HazardAlien alien)
        {
            //case we still want to destroy stunned aliens from alien projectiles?
            if (alien.StunnedFlag)
            {
                SafeDestroy(gameObject);
                return;
            }
        }

        if (!destroyTerrHazards) { return; }

        SafeDestroy(hazard.gameObject);
        MadeImpact();
    }

    public void OnEnteredBoss(AAlienBossBase boss)
    {
        //Null check on the sourceHitBox for bullets instanced from player
        if (boss.HitBox().RootParent == sourceHitBox?.RootParent) { return; }

        boss.TakeDamage(damageSettings.GetWeaponDamage(weaponType, damage2PlayerOrAlien: false));
        MadeImpact();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAlienProjectile) { return; }

        BoxColliderSP hb = other.gameObject.GetComponent<BoxColliderSP>();
        PlayerRunner player = hb?.RootParent?.GetComponent<PlayerRunner>();

        if (player != null)
        {
            //We give the player the audio so its from their source,
            //and so we don't prematurely delete this source object
            player.OnEnterProjectile(
                weaponType,
                requiredAvoidAction,
                /*impactAudio, */
                out bool dodged,
                out bool _);

            if (!dodged) { MadeImpact(); }
            return;
        }
    }

    private void MadeImpact()
    {
        impactAudio?.PlayAudioWrapper(audioSource);

        if (OnImpactPrefab != null)
        {
            GameObject instance = Instantiate(OnImpactPrefab, transform.parent);
            instance.transform.position = transform.position;
        }

        if (destroyOnImpact)
        {
            SafeDestroy(gameObject);
        }

        destructionSpritePrefab?.InstantiateDestruction(transform.parent, spriteAnim.transform);
    }
}
