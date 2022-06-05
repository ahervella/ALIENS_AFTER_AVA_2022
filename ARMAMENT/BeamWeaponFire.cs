using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamWeaponFire : WeaponFire
{
    protected override void InstantiateProjectile(Projectile projectile, Transform prefabParent, Vector3 projectilePos, Transform muzzleFlashTransform, BoxColliderSP shooterHitBox)
    {
        //Need this so we can tell the mz when to stop looping (can't use DSOs without passing around
        //a unique id if there will be multiple beams of any weapon
        base.InstantiateProjectile(projectile, prefabParent, projectilePos, muzzleFlashTransform, shooterHitBox);
        ((BeamProjectile)projectileInstance).SetBeamMuzzleFlashRef((BeamMuzzleFlash)muzzleFlashInstance);
    }
}
