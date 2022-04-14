using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField]
    private SO_ShooterSettings settings = null;

    [SerializeField]
    private Transform spawnRef = null;

    [SerializeField]
    private AnimationEventExtender aeExtender = null;

    private Coroutine fireCR = null;

    private ShooterWrapper cachedShooterWrapper;

    public static Shooter InstantiateShooterObject(Transform shooterParentRef, Vector3 shooterPos, Transform bulletSpawnRef, SO_ShooterSettings settings)
    {
        Shooter instance = new GameObject("INSTANCED_SHOOTER").AddComponent<Shooter>();
        instance.transform.transform.parent = shooterParentRef;
        instance.transform.position = shooterPos;

        instance.spawnRef = bulletSpawnRef;
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
            GameObject weaponPrefab = Instantiate(cachedShooterWrapper.WeaponPrefab, spawnRef);
            weaponPrefab.transform.position = transform.position;
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
