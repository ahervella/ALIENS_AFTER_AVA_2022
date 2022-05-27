using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private Vector3 projectileSpawnPos;

    [SerializeField]
    private BoxColliderSP shooterHitBox = null;

    [SerializeField]
    private AnimationEventExtender aeExtender = null;

    private Coroutine fireCR = null;

    private ShooterWrapper cachedShooterWrapper;

    private bool usingCustomShooterWrapper = false;

    public static Shooter InstantiateShooterObject(
        Transform shooterParentRef,
        Transform muzzleFlashPosRef,
        Vector3 projectilePos,
        BoxColliderSP shooterHitBox,
        ShooterWrapper customShooterWrapper)
    {
        Shooter instance = new GameObject("INSTANCED_SHOOTER").AddComponent<Shooter>();
        instance.transform.parent = shooterParentRef;

        instance.muzzleFlashSpawnPosRef = muzzleFlashPosRef;
        instance.projectileSpawnPos = projectilePos;
        instance.shooterHitBox = shooterHitBox;
        instance.usingCustomShooterWrapper = true;
        instance.cachedShooterWrapper = customShooterWrapper;
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
        while (true)
        {
            WeaponFire.InstantiateWithCustomPositions(
                cachedShooterWrapper.WeaponFirePrefab,
                transform,
                muzzleFlashSpawnPosRef,
                projectileSpawnPosRef?.position?? projectileSpawnPos,
                shooterHitBox);
            yield return new WaitForSeconds(cachedShooterWrapper.DelayTime);
        }
    }

    private void OnDestroy()
    {
        if (fireCR != null)
        {
            StopCoroutine(fireCR);
        }
    }
}
