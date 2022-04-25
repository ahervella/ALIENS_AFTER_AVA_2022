using System.Collections;
using System.Collections.Generic;
using PowerTools;
using UnityEngine;
using static HelperUtil;

[RequireComponent(typeof(SafeAudioWrapperSource))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    [SerializeField]
    private Vector3PropertySO hitBoxDimEdgePerc = null;

    [SerializeField]
    private PSO_TerrainTreadmillNodes terrTreadmillNodesPSO = null;

    [SerializeField]
    private bool spawnAttachedToTreadmillHorizontal = true;

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
    private float angleOffset = 0f;
    [SerializeField]
    private Vector3 posOffset = default;
    private Vector3 slope;

    [SerializeField]
    private float speedPerSec = 10;

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

    private AudioWrapperSource audioSource;

    private Transform mzTrans;

    private void Awake()
    {
        slope = (isAlienProjectile ? -1 : 1) * speedPerSec * new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * angleOffset),
            0,
            Mathf.Cos(Mathf.Deg2Rad * angleOffset));

        audioSource = GetComponent<AudioWrapperSource>();
        travelAudio?.PlayAudioWrapper(audioSource);

        SetMuzzleFlashTranform(transform);
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
        hitBox.Box().isTrigger = !isAlienProjectile;
        SetHitBoxDimensionsAndPos(
            hitBox.Box(),
            new Vector2(1, 1),
            terrSettings,
            hitBoxDimEdgePerc);


        hitBox.SetOnTriggerEnterMethod(OnTriggerEnter);
    }

    private void SetSpawnPosition()
    {
        if (spawnAttachedToTreadmillHorizontal)
        {
            transform.parent = terrTreadmillNodesPSO.Value.HorizontalNode;
        }

        float xPos = autoAlignToNearestLane ?
            GetLaneXPosition(GetLaneIndexFromPosition(transform.position.x, terrSettings), terrSettings)
            : transform.position.x;

        float yPos = 0;// GetFloorYPosition(FloorIndex, terrSettings);

        
        transform.position = new Vector3(xPos, yPos, transform.position.z);

        Vector3 originalSpriteLocalPos = mzTrans.position - transform.position;

        float spriteFinalYLocalPos = terrSettings.FloorHeight * spriteYPosPercFloorHeight;// + yPos;
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

    private void Update()
    {
        transform.position += slope * Time.deltaTime;
        CheckIfOutOfVerticalBounds();
    }

    private void CheckIfOutOfVerticalBounds()
    {
        //Destroy if one row behind 0 (which is when rows reset for terrNode)
        //or if further than last row
        if (transform.position.z > terrSettings.TileRows * terrSettings.TileDims.y
            || transform.position.z < -terrSettings.TileDims.y)
        {
            SafeDestroy(gameObject);
        }
    }

    public void OnEnteredHazard(TerrHazard hazard)
    {
        if (isAlienProjectile) { return; }

        SafeDestroy(hazard.gameObject);
        MadeImpact();
    }

    public void OnEnteredBoss(AAlienBossBase boss)
    {
        if (isAlienProjectile) { return; }

        boss.TakeDamage(1);
        MadeImpact();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAlienProjectile) { return; }

        PlayerRunner player = other.gameObject.GetComponent<PlayerRunner>();
        if (player != null)
        {
            //We give the player the audio so its from their source,
            //and so we don't prematurely delete this source object
            player.OnEnterProjectile(weaponType, requiredAvoidAction, /*impactAudio, */out bool dodged);

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
            destructionSpritePrefab?.InstantiateDestruction(transform.parent, spriteAnim.transform);
            SafeDestroy(gameObject);
        }
    }
}
