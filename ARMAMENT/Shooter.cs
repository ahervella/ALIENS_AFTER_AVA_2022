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
    private Vector3 muzzleFlashSpawnPos;

    [SerializeField]
    private Transform projectileSpawnPosRef = null;
    private Vector3 projectileSpawnPos;

    [SerializeField]
    private AnimationEventExtender aeExtender = null;

    private Coroutine fireCR = null;

    private ShooterWrapper cachedShooterWrapper;

    public static Shooter InstantiateShooterObject(Transform shooterParentRef, Vector3 muzzleFlashPos, Vector3 projectilePos, SO_ShooterSettings settings)
    {
        Shooter instance = new GameObject("INSTANCED_SHOOTER").AddComponent<Shooter>();
        instance.transform.transform.parent = shooterParentRef;

        instance.muzzleFlashSpawnPos = muzzleFlashPos;
        instance.projectileSpawnPos = projectilePos;
        instance.settings = settings;
        instance.AE_StartFiring();
        return instance;
    }

    private void Awake()
    {
        aeExtender?.AssignAnimationEvent(AE_StartFiring, 0);
    }

    private void AE_StartFiring()
    {
        cachedShooterWrapper = settings.GetRandShooterWrapper();
        fireCR = StartCoroutine(FireCoroutine());
    }

    public IEnumerator FireCoroutine()
    {
        while (true)
        {
            WeaponFire.InstantiateWithCustomPositions(
                cachedShooterWrapper.WeaponFirePrefab,
                transform,
                muzzleFlashSpawnPosRef?.position?? muzzleFlashSpawnPos,
                projectileSpawnPosRef?.position?? projectileSpawnPos);
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
