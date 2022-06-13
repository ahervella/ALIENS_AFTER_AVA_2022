using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

[CreateAssetMenu(fileName = "SO_TerrZoneWrapperSettings", menuName = "ScriptableObjects/StaticData/SO_TerrZoneWrapperSettings")]
public class SO_TerrZoneWrapperSettings : ScriptableObject
{
    [SerializeField]
    private List<SO_TerrZoneWrapper> wrappers = new List<SO_TerrZoneWrapper>();
    
    public SO_TerrZoneWrapper GetZoneWrapper(int zone)
    {
        return GetWrapperFromFunc(wrappers, tzw => tzw.Zone, zone, LogEnum.ERROR, null);
    }
}
