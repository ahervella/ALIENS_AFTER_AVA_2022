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

    private Transform prefabParent;
    private Vector3 muzzleFlashPos;
    private MuzzleFlash muzzleFlashInstance;

    private void Awake()
    {
        prefabParent = transform.parent;
        muzzleFlashPos = transform.position;

        muzzleFlashInstance = InstantiateAndSetPosition(muzzleFlash, prefabParent, muzzleFlashPos);

        CustomizePositions(prefabParent, transform.position, transform.position);
        StartCoroutine(DestroyAtEndOfFrame());
    }

    private void CustomizePositions(Transform prefabParent, Vector3 muzzleFlashPos, Vector3 projectilePos)
    {
        muzzleFlashInstance.transform.parent = prefabParent;
        muzzleFlashInstance.transform.position = muzzleFlashPos;
        muzzleFlashInstance.SetProjectileFireMethod(() => InstantiateAndSetPosition(projectile, prefabParent, projectilePos));
    }

    public static void InstantiateWithCustomPositions(WeaponFire weaponFirePrefab, Transform prefabParent, Vector3 muzzleFlashPos, Vector3 projectilePos)
    {
        WeaponFire instance = GameObject.Instantiate(weaponFirePrefab, prefabParent);
        instance.CustomizePositions(prefabParent, muzzleFlashPos, projectilePos);
    }

    private IEnumerator DestroyAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        SafeDestroy(gameObject);
    }
}
