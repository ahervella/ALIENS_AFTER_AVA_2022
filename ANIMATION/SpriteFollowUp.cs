using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent(typeof(SpriteAnim))]
public class SpriteFollowUp : MonoBehaviour
{
    [SerializeField]
    private AnimationClip followUpAnimation = null;

    public void AE_OnAnimFollowUp(AnimationClip ac)
    {
        if (ac == null)
        {
            ac = followUpAnimation;
        }
        GetComponent<SpriteAnim>().Play(ac);
    }
}
