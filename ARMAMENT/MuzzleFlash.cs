using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

public class MuzzleFlash : MonoBehaviour
{
    private Action projectileMethod;

    public void SetProjectileFireMethod(Action projectileMethod)
    {
        this.projectileMethod = projectileMethod;
    }

    public void AE_FireProjectile()
    {
        projectileMethod?.Invoke();
    }

    public void AE_OnAnimationFinished()
    {
        SafeDestroy(gameObject);
    }
}
