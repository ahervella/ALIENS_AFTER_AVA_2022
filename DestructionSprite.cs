using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

[RequireComponent(typeof(SpriteAnim))]
public class DestructionSprite : MonoBehaviour
{
    [SerializeField]
    GameObject object2Destroy = null;

    public void AE_OnAnimationFinished()
    {
        Destroy(object2Destroy == null ? gameObject : object2Destroy);
    }

    public DestructionSprite InstantiateDestruction(Transform parent, Transform spawnPosRot)
    {
        while (parent == null)
        {
            parent = parent.parent;
        }

        DestructionSprite instance = Instantiate(this, parent);
        instance.transform.SetPositionAndRotation(spawnPosRot.position, spawnPosRot.rotation);
        return this;
    }
}
