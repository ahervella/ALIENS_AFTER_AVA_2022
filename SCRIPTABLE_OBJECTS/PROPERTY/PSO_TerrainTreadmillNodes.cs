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
    public Transform HorizontalNode { get; private set; }
    public Transform VerticalNode { get; private set; }

    public TerrainTreadmillNodesWrapper(
        Transform HorizontalNode, Transform VerticalNode)
    {
        this.HorizontalNode = HorizontalNode;
        this.VerticalNode = VerticalNode;
    }
}
