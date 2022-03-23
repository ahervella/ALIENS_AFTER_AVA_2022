using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField]
    private Projectile projectile = null;
   
    public void AE_FireProjectile()
    {
        Projectile instance = Instantiate(projectile, transform.parent);
        instance.transform.localPosition = transform.localPosition;
    }

    public void AE_OnAnimationFinished()
    {
        SafeDestroy(gameObject);
    }
}
