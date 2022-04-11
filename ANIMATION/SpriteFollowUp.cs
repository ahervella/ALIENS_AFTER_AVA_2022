using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent(typeof(SpriteAnim))]
public class SpriteFollowUp : MonoBehaviour
{
    [SerializeField]
    private AnimationClip followUpAnimation = null;

    public void AE_OnAnimFollowUp()
    {
        GetComponent<SpriteAnim>().Play(followUpAnimation);
    }

    public void AE_OnAnimFollowUp(AnimationClip ac)
    {
        GetComponent<SpriteAnim>().Play(ac);
    }
}
