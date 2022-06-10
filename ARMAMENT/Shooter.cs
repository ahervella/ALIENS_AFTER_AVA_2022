using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

public class Shooter : MonoBehaviour
{
    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    [SerializeField]
    private SO_ShooterSettings settings = null;

    [SerializeField]
    private Transform muzzleFlashSpawnPosRef = null;

    [SerializeField]
    private Transform projectileSpawnPosRef = null;
    private Func<Vector3> projectileSpawnPosFunc;

    [SerializeField]
    private BoxColliderSP shooterHitBox = null;

    [SerializeField]
    private AnimationEventExtender aeExtender = null;

    private Coroutine fireCR = null;

    private ShooterWrapper cachedShooterWrapper;

    private bool usingCustomShooterWrapper = false;

    private int numberOfShots = -1;

    public static Shooter InstantiateShooterObject(
        Transform shooterParentRef,
        Transform muzzleFlashPosRef,
        Func<Vector3> projectilePosFunc,
        BoxColliderSP shooterHitBox,
        ShooterWrapper customShooterWrapper,
        int numberOfShots = -1)
    {
        Shooter instance = new GameObject("INSTANCED_SHOOTER").AddComponent<Shooter>();
        instance.transform.parent = shooterParentRef;

        instance.muzzleFlashSpawnPosRef = muzzleFlashPosRef;
        instance.projectileSpawnPosFunc = projectilePosFunc;
        instance.shooterHitBox = shooterHitBox;
        instance.usingCustomShooterWrapper = true;
        instance.cachedShooterWrapper = customShooterWrapper;
        instance.numberOfShots = numberOfShots;
        instance.AE_StartFiring();
        return instance;
    }

    private void Awake()
    {
        aeExtender?.AssignAnimationEvent(AE_StartFiring, 0);
    }

    private void AE_StartFiring()
    {
        if (!usingCustomShooterWrapper)
        {
            cachedShooterWrapper = settings.GetRandShooterWrapper();
        }
        fireCR = StartCoroutine(FireCoroutine());
    }

    public IEnumerator FireCoroutine()
    {
        while (numberOfShots == -1 || numberOfShots > 0)
        {
            WeaponFire.InstantiateWithCustomPositions(
                cachedShooterWrapper.WeaponFirePrefab,
                transform.parent,
                muzzleFlashSpawnPosRef,

                //Find a way around having to feed it a relative position (maybe instead
                //give it a relative lane?) to be able to auto align when the prefab fires
                //And to stop getting that bug of when a shooter class starts multiple shots
                //it can adapt based on the local position of the muzzle flash (maybe fire the
                //projectile and use the position relative to the muzzle flash?)

                //1) make a method for designating that lane moved
                //2) automatically move lanes  



                projectileSpawnPosRef?.position?? projectileSpawnPosFunc(),
                shooterHitBox);

            if (numberOfShots != -1) { numberOfShots--; }
            yield return new WaitForSeconds(cachedShooterWrapper.DelayTime);
        }
        SafeDestroy(gameObject);
    }
}
