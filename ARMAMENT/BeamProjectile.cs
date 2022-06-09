using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using PowerTools;

public class BeamProjectile : Projectile
{
    [SerializeField]
    private Transform beamRotTrans = null;

    [SerializeField]
    private int tileDistLong = default;

    [SerializeField]
    private int beamTileDistLong = default;

    [SerializeField]
    private float beamTargetFloorHeightPos = default;

    private Vector3 cachedTargetPos;

    [SerializeField]
    private float beamDuration = 3f;

    [SerializeField]
    private SpriteRenderer spriteRend = null;

    [SerializeField]
    private AnimationClip beamEndAnim = null;

    [SerializeField]
    private AnimationEventExtender aee = null;

    private BeamMuzzleFlash mzRef;

    private Coroutine beamEndCR = null;

    protected override void OnAwake()
    {
        base.OnAwake();
        hitBox.BoxDisabled = true;
        aee.AssignAnimationEvent(AE_DestroyBeam, 0);
        aee.AssignAnimationEvent(AE_ActivateBeamHitBox, 1);
    }

    protected override void ConfigHitBox()
    {
        //TODO: better way to clarify this with prefab setup or here? Especially given that
        //programmer might not notice the local position of the hitbox is set in the SetHitBoxDimensionsAndPos
        //via the hb UseLocalPos field?

        //alien projectiles (aka only the boss), will need to rotate with the beam
        //otherwise the hitbox stays static down the player lane
        if (!autoAlignToNearestLane)
        {
            hitBox.transform.parent = beamRotTrans;
        }
        else
        {
            hitBox.transform.parent = transform;
        }

        hitBox.Box().isTrigger = true;
        SetHitBoxDimensionsAndPos(
            hitBox,
            //TODO: not sure why the hit box is still a wee bit longer than
            //the beam but whatever lol
            new Vector2(widthHeightDims.x, tileDistLong),
            widthHeightDims.y,
            terrSettings,
            hitBoxDimEdgePerc);

        OffsetHitBoxCenterToEdge(hitBox, towardsOrAwayFromPlayer: false);//isAlienProjectile);

        hitBox.SetOnTriggerEnterMethod(OnTriggerEnter);
    }

    protected override void SetSpawnPosition()
    {
        //base.SetSpawnPosition();

        if(autoAlignToNearestLane)
        {
            Vector3 rotOriginalPos = beamRotTrans.position;

            //for hitbox
            transform.position = new Vector3(
               GetLaneXPosition(GetLaneIndexFromPosition(transform.position.x, terrSettings), terrSettings),
                transform.position.y,
                transform.position.z
            );

            beamRotTrans.position = rotOriginalPos;
        }

        cachedTargetPos = new Vector3(
        transform.position.x,
        beamTargetFloorHeightPos * terrSettings.FloorHeight,
        beamTileDistLong * terrSettings.TileDims.y
        );

        if(autoAlignToNearestLane)
        {
            UpdateBeamRotation(); 
        }
        else{
             spriteRend.size = new Vector2(Vector3.Distance(mzTrans.position, cachedTargetPos), spriteRend.size.y);
        }

        
    }

    private void Update()
    {
        if(isAlienProjectile) { return; }
        UpdateBeamRotation();    
    }

    private void UpdateBeamRotation()
    {
        beamRotTrans.LookAt(cachedTargetPos);
        spriteRend.size = new Vector2(Vector3.Distance(mzTrans.position, cachedTargetPos), spriteRend.size.y);
    }

    public void SetBeamMuzzleFlashRef(BeamMuzzleFlash mzRef)
    {
        this.mzRef = mzRef;
    }

    private void AE_ActivateBeamHitBox()
    {
        hitBox.BoxDisabled = false;
        SafeStartCoroutine(ref beamEndCR, BeamEndCR(), this);
    }

    private IEnumerator BeamEndCR()
    {
        yield return new WaitForSeconds(beamDuration);
        spriteAnim.Play(beamEndAnim);
        hitBox.BoxDisabled = true;
        mzRef.OnBeamDone();
    }

    private void AE_DestroyBeam()
    {
        SafeDestroy(gameObject);
    }

    private void OnDestroy()
    {
        SafeStopCoroutine(ref beamEndCR, this);
    }
}
