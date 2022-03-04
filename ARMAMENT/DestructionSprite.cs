using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent(typeof(SpriteAnim))]
public class DestructionSprite : MonoBehaviour
{
    public void AE_OnAnimationFinished()
    {
        Destroy(gameObject);
    }

    public DestructionSprite InstantiateDestruction(Transform parent)
    {
        while (parent == null)
        {
            parent = parent.parent;
        }

        DestructionSprite instance = Instantiate(this, transform.parent);
        instance.transform.SetPositionAndRotation(transform.position, transform.rotation);
        return this;
    }
}
