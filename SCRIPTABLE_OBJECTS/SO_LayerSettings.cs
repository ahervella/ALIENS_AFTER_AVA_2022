using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using System;

[CreateAssetMenu(fileName = "SO_LayerSettings", menuName = "ScriptableObjects/StaticData/SO_LayerSettings")]
public class SO_LayerSettings : ScriptableObject
{
    [SerializeField]
    private int hitBoxLayer = default;
    public int HitBoxLayer => hitBoxLayer;

    //TODO: delete this if we don't end up needing something more complicated

    //[SerializeField]
    //private List<LayerWrapper> layerWrappers = new List<LayerWrapper>();

    //public int GetLayerInt(LayerEnum layerType)
    //{
    //    return GetWrapperFromFunc(
    //        layerWrappers,
    //        lw => lw.LayerType,
    //        layerType,
    //        LogEnum.ERROR,
    //        null).LayerInt;
    //}


    //public Type GetLayerClassType(LayerEnum layerType)
    //{
    //    switch (layerType)
    //    {
    //        case LayerEnum.ALIEN:
    //        case LayerEnum.DEFAULT_HAZARD:
    //        case LayerEnum.JUMPABLE_HAZARD:
    //            return typeof(TerrHazard);

    //        case LayerEnum.PROJECTILE:
    //            return typeof(Projectile);

    //        default:
    //            Debug.LogError("No class type set for this layer type!");
    //            return null;
    //    }
    //}

    //[Serializable]
    //private class LayerWrapper
    //{
    //    [SerializeField]
    //    private LayerEnum layerType = LayerEnum.DEFAULT_HAZARD;
    //    public LayerEnum LayerType => layerType;

    //    [SerializeField]
    //    private int layerInt = 0;
    //    public int LayerInt => layerInt;
    //}
}

//public enum LayerEnum
//{
//    ALIEN = 0, JUMPABLE_HAZARD = 1, DEFAULT_HAZARD = 2, PROJECTILE = 3
//}
