﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using PowerTools;
using static AudioUtil;

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
    private float cachedTargetTweenTime = 1f;

    [SerializeField]
    private float highShotExtraRawHeight = 10f;

    [SerializeField]
    private SpriteRenderer spriteRend = null;

    [SerializeField]
    private SpriteRenderer beamTipSprite = null;

    [SerializeField]
    private AnimationClip beamEndAnim = null;

    [SerializeField]
    private AnimationEventExtender aee = null;

    [SerializeField]
    private AAudioWrapperV2 chargeAudio = null;

    [SerializeField]
    private AAudioWrapperV2 fireAudio = null;

    [SerializeField]
    private AAudioWrapperV2 beamDoneAudio = null;

    [SerializeField]
    private BoolPropertySO optionalBeamActivePSO = null;

    private BeamMuzzleFlash mzRef;

    private Coroutine beamEndCR = null;

    private Coroutine targetTweenCR = null;

    protected override void OnAwake()
    {
        base.OnAwake();
        hitBox.BoxDisabled = true;
        beamTipSprite.enabled = false;
        aee.AssignAnimationEvent(AE_DestroyBeam, 0);
        aee.AssignAnimationEvent(AE_ActivateBeamHitBox, 1);
        chargeAudio.PlayAudioWrapper(audioSource);
    }

    protected override void Start()
    {
        base.Start();

        if (!isAlienProjectile)
        {
            currPlayerAction.RegisterForPropertyChanged(OnPlayerActionChange);
        }
    }

    private void OnPlayerActionChange(PlayerActionEnum oldAction, PlayerActionEnum newAction)
    {
        if (oldAction == PlayerActionEnum.JUMP || newAction == PlayerActionEnum.JUMP)
        {
            UpdateCachedTarget();
        }
    }

    private void UpdateCachedTarget(bool skipTween = false)
    {
        Vector3 newTargetPos = new Vector3(
        transform.position.x,
        beamTargetFloorHeightPos * terrSettings.FloorHeight + (HighOrLowShot ? highShotExtraRawHeight : 0),
        beamTileDistLong * terrSettings.TileDims.y
        );
        if (skipTween)
        {
            cachedTargetPos = newTargetPos;
            return;
        }

        SafeStartCoroutine(ref targetTweenCR, TweenNewCachedTarget(newTargetPos), this);
    }

    private IEnumerator TweenNewCachedTarget(Vector3 newTargetPos)
    {
        float perc = 0;
        Vector3 oldTargetPos = cachedTargetPos;

        while (perc < 1)
        {
            perc += Time.deltaTime / cachedTargetTweenTime;
            cachedTargetPos = Vector3.Lerp(oldTargetPos, newTargetPos, EasedPercent(perc));
            yield return null;
        }
        targetTweenCR = null;
    }

    protected override void ConfigHitBox()
    {
        //TODO: better way to clarify this with prefab setup or here? Especially given that
        //programmer might not notice the local position of the hitbox is set in the SetHitBoxDimensionsAndPos
        //via the hb UseLocalPos field?

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

        UpdateCachedTarget(skipTween: true);

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
        float beamDist = Vector3.Distance(mzTrans.position, cachedTargetPos);
        spriteRend.size = new Vector2(beamDist, spriteRend.size.y);
        beamTipSprite.transform.localPosition = new Vector3(
            -beamDist,
            beamTipSprite.transform.localPosition.y,
            beamTipSprite.transform.localPosition.z);
    }

    public void SetBeamMuzzleFlashRef(BeamMuzzleFlash mzRef)
    {
        this.mzRef = mzRef;
    }

    private void AE_ActivateBeamHitBox()
    {
        hitBox.BoxDisabled = false;
        beamTipSprite.enabled = true;
        fireAudio.PlayAudioWrapper(audioSource);
        SafeStartCoroutine(ref beamEndCR, BeamEndCR(), this);
    }

    private IEnumerator BeamEndCR()
    {
        yield return new WaitForSeconds(beamDuration);
        spriteAnim.Play(beamEndAnim);
        hitBox.BoxDisabled = true;
        mzRef.OnBeamDone();
        
        //TODO: for what ever reason, if the beamDoneAudio was not
        //in an audio event wrapper (ie. just the audio clip wrapper)
        //it would not play here....
        StopAllAudioSourceSounds(audioSource);
        beamDoneAudio.PlayAudioWrapper(audioSource);

        beamEndCR = null;
    }

    private void AE_DestroyBeam()
    {
        SafeDestroy(gameObject);
    }

    private void OnDestroy()
    {
        optionalBeamActivePSO?.ModifyValue(false);
        currPlayerAction.DeRegisterForPropertyChanged(OnPlayerActionChange);
        SafeStopCoroutine(ref beamEndCR, this);
    }
}
