using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent(typeof(SpriteAnim))]
public class SpriteFollowUp : MonoBehaviour
{
    [SerializeField]
    private AnimationClip followUpAnimation = null;

    public void AE_OnAnimationFinished()
    {
        GetComponent<SpriteAnim>().Play(followUpAnimation);
    }
}
