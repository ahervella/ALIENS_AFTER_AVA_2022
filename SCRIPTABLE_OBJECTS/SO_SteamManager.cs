using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

[CreateAssetMenu(fileName = "SO_SteamManager", menuName = "ScriptableObjects/StaticData/SO_SteamManager")]
public class SO_SteamManager : ScriptableObject
{
    public bool TryCompleteAchievement(SteamAchievementsEnum sae)
    {
        if (sae == SteamAchievementsEnum.NONE) { return false; }
        if (!SteamManager.Initialized) { return false; }

        SteamUserStats.SetAchievement(sae.ToString());
        SteamUserStats.StoreStats();
        return true;
    }
}

public enum SteamAchievementsEnum
{
    NONE = 0,
    ZONE_1_RUN = 1,
    ZONE_1_BOSS = 2,
    ZONE_2_RUN = 3,
    ZONE_2_BOSS = 4,
    ZONE_3_RUN = 5,
    ZONE_3_BOSS = 6
}
