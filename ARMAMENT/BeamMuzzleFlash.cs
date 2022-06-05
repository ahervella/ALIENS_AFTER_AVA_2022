using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

public class BeamMuzzleFlash : MuzzleFlash
{
    [SerializeField]
    private AnimationClip muzzleFlashEnd = null;

    [SerializeField]
    private SpriteAnim anim = null;
    
    public void OnBeamDone()
    {
        anim.Play(muzzleFlashEnd);
    }
}
