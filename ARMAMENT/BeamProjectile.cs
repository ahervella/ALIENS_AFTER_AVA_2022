using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using PowerTools;

public class BeamProjectile : Projectile
{
    [SerializeField]
    private int tileDistLong = default;

    [SerializeField]
    private float beamDuration = 3f;

    [SerializeField]
    private SpriteRenderer spriteRend = null;

    [SerializeField]
    private AnimationClip beamEndAnim = null;

    [SerializeField]
    private AnimationEventExtender aee = null;

    private BeamMuzzleFlash mzRef;

    protected override void OnAwake()
    {
        base.OnAwake();
        hitBox.BoxDisabled = true;
        aee.AssignAnimationEvent(AE_DestroyBeam, 0);
        aee.AssignAnimationEvent(AE_ActivateBeamHitBox, 1);
    }

    protected override void ConfigHitBox()
    {
        hitBox.Box().isTrigger = true;
        SetHitBoxDimensionsAndPos(
            hitBox,
            //TODO: not sure why the hit box is still a wee bit longer than
            //the beam but whatever lol
            new Vector2(widthHeightDims.x, tileDistLong),
            widthHeightDims.y,
            terrSettings,
            hitBoxDimEdgePerc);

        OffsetHitBoxCenterToEdge(hitBox, towardsOrAwayFromPlayer: isAlienProjectile);

        hitBox.SetOnTriggerEnterMethod(OnTriggerEnter);
    }

    protected override void SetSpawnPosition()
    {
        base.SetSpawnPosition();

        //using the correct rotations set up in prefab and just adjusting
        //to face from the muzzle to the end point
        float xEndPoint = GetLaneXPosition(GetLaneIndexFromPosition(transform.position.x, terrSettings), terrSettings);
        float yEndPoint = terrSettings.FloorHeight / 2f;
        float zEndPoint = tileDistLong * terrSettings.TileDims.y;

        Vector3 endPoint = new Vector3(xEndPoint, yEndPoint, zEndPoint);
        Vector3 startPoint = mzTrans.position;

        float xLength = xEndPoint - startPoint.x;
        float zLength = zEndPoint - startPoint.z;

        //assuming pivot is from startPoint
        float length = Mathf.Sqrt(Mathf.Pow(zLength, 2f) + Mathf.Pow(xLength, 2f));
        spriteRend.size = new Vector2(length, spriteRend.size.y);

        Quaternion localRot = spriteRend.transform.localRotation;
        Vector3 eulerAngles = localRot.eulerAngles;
        localRot.SetLookRotation(endPoint - startPoint);

        Vector3 rot = eulerAngles + localRot.eulerAngles;
        spriteRend.transform.localRotation = Quaternion.Euler(rot);
    }

    public void SetBeamMuzzleFlashRef(BeamMuzzleFlash mzRef)
    {
        this.mzRef = mzRef;
    }

    private void AE_ActivateBeamHitBox()
    {
        hitBox.BoxDisabled = false;
        StartCoroutine(BeamEndCR());
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
}
