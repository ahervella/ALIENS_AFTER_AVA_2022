using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class WeaponFire : MonoBehaviour
{
    [SerializeField]
    private MuzzleFlash muzzleFlash = null;

    [SerializeField]
    private Projectile projectile = null;

    private BoxColliderSP shooterHitBox;

    private MuzzleFlash muzzleFlashInstance;

    private void Awake()
    {
        muzzleFlashInstance = Instantiate(muzzleFlash, transform.parent);

        CustomizePositions(transform.parent, transform.parent, transform.position);
        StartCoroutine(DestroyAtEndOfFrame());
    }

    private void CustomizePositions(Transform prefabParent, Transform muzzleFlashPosRef, Vector3 projectilePos)
    {
        muzzleFlashInstance.transform.parent = muzzleFlashPosRef;
        muzzleFlashInstance.transform.localPosition = Vector3.zero;
        muzzleFlashInstance.SetProjectileFireMethod(() => InstantiateProjectile(
            projectile,
            prefabParent,
            projectilePos,
            muzzleFlashPosRef,
            shooterHitBox));
    }

    private static void InstantiateProjectile(Projectile projectile, Transform prefabParent, Vector3 projectilePos, Transform muzzleFlashTransform, BoxColliderSP shooterHitBox)
    {
        Projectile instance = InstantiateAndSetPosition(projectile, prefabParent, projectilePos);
        instance.SetMuzzleFlashTranform(muzzleFlashTransform);
        instance.SetFireSourceHitBox(shooterHitBox);
    }

    public static void InstantiateWithCustomPositions(WeaponFire weaponFirePrefab, Transform prefabParent, Transform muzzleFlashPosRef, Vector3 projectilePos, BoxColliderSP shooterHitBox)
    {
        WeaponFire instance = GameObject.Instantiate(weaponFirePrefab, prefabParent);
        instance.shooterHitBox = shooterHitBox;
        instance.CustomizePositions(prefabParent, muzzleFlashPosRef, projectilePos);
    }

    private IEnumerator DestroyAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        SafeDestroy(gameObject);
    }
}
