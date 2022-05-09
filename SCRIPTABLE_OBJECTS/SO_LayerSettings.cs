using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using System;

[CreateAssetMenu(fileName = "SO_LayerSettings", menuName = "ScriptableObjects/StaticData/SO_LayerSettings")]
public class SO_LayerSettings : ScriptableObject
{
    [SerializeField]
    private List<LayerWrapper> layerWrappers = new List<LayerWrapper>();

    public int GetLayerInt(LayerEnum layerType)
    {
        return GetWrapperFromFunc(
            layerWrappers,
            lw => lw.LayerType,
            layerType,
            LogEnum.ERROR,
            null).LayerInt;
    }

    public List<int> GetAllLayers()
    {
        List<int> list = new List<int>();
        foreach (LayerWrapper lw in layerWrappers)
        {
            list.Add(lw.LayerInt);
        }
        return list;
    }

    [Serializable]
    private class LayerWrapper
    {
        [SerializeField]
        private LayerEnum layerType = LayerEnum.DEFAULT_HAZARD;
        public LayerEnum LayerType => layerType;

        [SerializeField]
        private int layerInt = 0;
        public int LayerInt => layerInt;
    }
}

public enum LayerEnum
{
    ALIEN = 0, JUMPABLE_HAZARD = 1, DEFAULT_HAZARD = 2
}
