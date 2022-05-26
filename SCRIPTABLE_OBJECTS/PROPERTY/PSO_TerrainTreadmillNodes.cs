using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_TerrainTreadmillNodes", menuName = "ScriptableObjects/Property/PSO_TerrainTreadmillNodes")]
public class PSO_TerrainTreadmillNodes : PropertySO<TerrainTreadmillNodesWrapper>
{
    public override void ModifyValue(TerrainTreadmillNodesWrapper mod)
    {
        SetValue(mod);
    }
}

public class TerrainTreadmillNodesWrapper
{
    private Transform horizontalNode;
    private Transform verticalNode;

    public TerrainTreadmillNodesWrapper(
        Transform horizontalNode, Transform verticalNode)
    {
        this.horizontalNode = horizontalNode;
        this.verticalNode = verticalNode;
    }

    public void AttachTransform(Transform trans, bool horizOrVert, bool useContainer = false)
    {
        Transform node = horizOrVert ? horizontalNode : verticalNode;

        if (!useContainer)
        {
            trans.parent = node.transform;
            return;
        }

        GameObject newContainer = new GameObject(trans.name + " CONTAINER");

        newContainer.transform.parent = node;
        newContainer.transform.localPosition = Vector3.zero;

        trans.parent = newContainer.transform;
    }

    public void DettachTransform(Transform trans, Transform newParent, bool usedContainer = false)
    {
        if (!usedContainer)
        {
            trans.parent = newParent;
            return;
        }

        Transform container = trans.parent;
        trans.parent = newParent;

        GameObject.Destroy(container.gameObject);
    }
}
