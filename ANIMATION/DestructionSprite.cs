using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;
using static HelperUtil;

[RequireComponent(typeof(SpriteAnim))]
public class DestructionSprite : MonoBehaviour
{
    [SerializeField]
    private GameObject object2Destroy = null;

    [SerializeField]
    private PSO_TerrainTreadmillNodes terrNodesPSO = null;

    [SerializeField]
    private bool attach2Treadmill = true;

    public void AE_OnAnimationFinished()
    {
        SafeDestroy(object2Destroy == null ? gameObject : object2Destroy);
    }

    public DestructionSprite InstantiateDestruction(Vector3 spawnPos, Quaternion spawnRot = new Quaternion())
    {
        DestructionSprite instance = Instantiate(this);
        if (attach2Treadmill)
        {
            terrNodesPSO.Value.AttachTransform(instance.transform, horizOrVert: false);
        }
        instance.transform.SetPositionAndRotation(spawnPos, spawnRot);
        return this;
    }
}
