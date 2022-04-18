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
    private BoxCollider hitBox = null;

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

    private void Awake()
    {
        slope = (isAlienProjectile ? -1 : 1) * speedPerSec * new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * angleOffset),
            0,
            Mathf.Cos(Mathf.Deg2Rad * angleOffset));

        audioSource = GetComponent<AudioWrapperSource>();
        travelAudio?.PlayAudioWrapper(audioSource);

        //TODO: is this too jank or can I just resort to using on collision or on trigger and return
        //depending on the isAlienProjectile flag?
        hitBox.isTrigger = !isAlienProjectile;
        SetHitBoxDimensionsAndPos(
            hitBox,
            new Vector2(1, 1),
            terrSettings,
            hitBoxDimEdgePerc);

        //TODO make part of instantiation change what floor its on
        float  yPos = terrSettings.FloorHeight * spriteYPosPercFloorHeight;
        spriteAnim.transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);

        transform.position += posOffset;
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
        PlayerRunner player = other.gameObject.GetComponent<PlayerRunner>();
        if (player != null)
        {
            //We give the player the audio so its from their source,
            //and so we don't prematurely delete this source object
            player.OnEnterProjectile(weaponType, requiredAvoidAction, /*impactAudio, */out bool dodged);

            if (dodged) { MadeImpact(); }
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
            destructionSpritePrefab?.InstantiateDestruction(transform.parent, transform);
            SafeDestroy(gameObject);
        }
    }
}
