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

    public static Shooter InstantiateShooterObject(Transform spawnParentRef, Vector3 offset, SO_ShooterSettings settings)
    {
        Shooter instance = new GameObject("INSTANCED_SHOOTER").AddComponent<Shooter>();
        instance.transform.transform.parent = spawnParentRef;
        instance.transform.localPosition = offset;

        //hack to make this spawn at the given spawnRef with the offset :)
        instance.spawnRef = instance.transform;
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
            Instantiate(cachedShooterWrapper.WeaponPrefab, spawnRef);
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
