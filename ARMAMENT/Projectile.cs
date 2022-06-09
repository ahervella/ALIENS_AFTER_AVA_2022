using System.Collections;
using System.Collections.Generic;
using PowerTools;
using UnityEngine;
using static HelperUtil;

[RequireComponent(typeof(SafeAudioWrapperSource))]
public class Projectile : MovingNode
{
    [SerializeField]
    protected Vector3PropertySO hitBoxDimEdgePerc = null;

    [SerializeField]
    private PSO_TerrainTreadmillNodes terrTreadmillNodesPSO = null;

    [SerializeField]
    protected Vector2Int widthHeightDims = new Vector2Int(1, 1);

    [SerializeField]
    private TREADMILL_ATTACHMENT treadmillAttachment = TREADMILL_ATTACHMENT.NONE;

    private enum TREADMILL_ATTACHMENT { NONE = 0, HORIZONTAL = 1, HORIZ_VERT = 2 }

    [SerializeField]
    protected BoxColliderSP hitBox = null;

    [SerializeField]
    protected SpriteAnim spriteAnim = null;

    [SerializeField]
    private WeaponEnum weaponType;

    [SerializeField]
    private PlayerActionEnum requiredAvoidAction = PlayerActionEnum.NONE;

    [SerializeField]
    private float spriteYPosPercFloorHeight = 0.5f;


    [SerializeField]
    private Vector3 posOffset = default;

    [SerializeField]
    protected bool autoAlignToNearestLane = true;

    [SerializeField]
    private bool alsoAutoAlignSprite = true;

    [SerializeField]
    private float timeSprite2Align2NearestLane = 0.5f;

    [SerializeField]
    private bool destroyOnImpact = true;

    //TODO: eventually handle the object being destroyed to have its own
    //on destruction aside from the projectile's destruction (ie. explosion)
    //[SerializeField]
    //private GameObject onImpactPrefab = null;

    [SerializeField]
    private DestructionSprite destructionSpritePrefab = null;

    [SerializeField]
    private AAudioWrapperV2 travelAudio = null;

    [SerializeField]
    private AAudioWrapperV2 impactAudio = null;

    [SerializeField]
    protected bool isAlienProjectile = false;

    [SerializeField]
    private bool destroyTerrHazards = true;

    [SerializeField]
    private PSO_CurrentPlayerAction currPlayerAction = null;

    [SerializeField]
    private SO_DamageQuantSettings damageSettings = null;

    private bool highOrLowShot =>
        !isAlienProjectile && currPlayerAction.Value == PlayerActionEnum.JUMP;

    private AudioWrapperSource audioSource;

    protected Transform mzTrans;

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
        //this order is important for beam projectiles
        //due to how the hitbox reattaches itself to the proper
        //parent and when it resets its local position

        //TODO: make it less jank somehow?
        SetSpawnPosition();
        ConfigHitBox();
    }

    protected virtual void ConfigHitBox()
    {
        //TODO: is this too jank or can I just resort to using on collision or on trigger and return
        //depending on the isAlienProjectile flag?
        hitBox.Box().isTrigger = true;//!isAlienProjectile;
        SetHitBoxDimensionsAndPos(
            hitBox,
            new Vector2(widthHeightDims.x, 1),
            widthHeightDims.y,
            terrSettings,
            hitBoxDimEdgePerc);


        hitBox.SetOnTriggerEnterMethod(OnTriggerEnter);
    }

    protected virtual void SetSpawnPosition()
    {
        Attach2TreadmillNodes();

        float xPos = autoAlignToNearestLane ?
            GetLaneXPosition(GetLaneIndexFromPosition(transform.position.x, terrSettings), terrSettings)
            : transform.position.x;

        float yPos = highOrLowShot ? terrSettings.FloorHeight : 0;//0;// GetFloorYPosition(FloorIndex, terrSettings);



        transform.position = new Vector3(xPos, yPos, transform.position.z);

        Vector3 originalSpriteLocalPos = mzTrans.position - transform.position;

        if (alsoAutoAlignSprite)
        {
            float spriteFinalYLocalPos = terrSettings.FloorHeight * spriteYPosPercFloorHeight + yPos;
            Vector3 finalLocalPos = new Vector3(0, spriteFinalYLocalPos, 0);

            StartCoroutine(SpritePosTween(originalSpriteLocalPos, finalLocalPos));
        }
        else
        {
            spriteAnim.transform.localPosition = originalSpriteLocalPos;
        }

        if (!autoAlignToNearestLane)
        {
            transform.position += posOffset;
        }
    }

    protected void Attach2TreadmillNodes()
    {
        if (treadmillAttachment == TREADMILL_ATTACHMENT.HORIZONTAL)
        {
            terrTreadmillNodesPSO.Value.AttachTransform(transform, horizOrVert: true);
        }
        else if (treadmillAttachment == TREADMILL_ATTACHMENT.HORIZ_VERT)
        {
            terrTreadmillNodesPSO.Value.AttachTransform(transform, horizOrVert: false);
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

    public void OnEnteredHazard(TerrHazard hazard, BoxColliderSP hazardhb)
    {
        //Null check on the sourceHitBox for bullets instanced from player
        if (hazardhb.RootParent == sourceHitBox?.RootParent) { return; }

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
        MadeImpact(hazardhb.transform.position.z);
    }

    public void OnEnteredBoss(AAlienBossBase boss)
    {
        //Null check on the sourceHitBox for bullets instanced from player
        if (boss.HitBox().RootParent == sourceHitBox?.RootParent) { return; }

        boss.TakeDamage(damageSettings.GetWeaponDamage(weaponType, damage2PlayerOrAlien: false));
        MadeImpact(boss.HitBox().transform.position.z);
    }

    protected void OnTriggerEnter(Collider other)
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

            if (!dodged) { MadeImpact(hb.transform.position.z); }
            return;
        }
    }

    private void MadeImpact(float impactSpawnZPos)
    {
        //to insure that we don't spawn the destruct prefab in
        //say the middle of a hazard instead where the bullet was in xy space
        Vector3 impactSpawnPos = new Vector3(
            spriteAnim.transform.position.x,
            spriteAnim.transform.position.y,
            impactSpawnZPos);

        impactAudio?.PlayAudioWrapper(audioSource);

        destructionSpritePrefab?.InstantiateDestruction(impactSpawnPos);

        if (destroyOnImpact)
        {
            SafeDestroy(gameObject);
        }
    }
}
