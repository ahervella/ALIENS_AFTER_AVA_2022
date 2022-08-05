using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System;

//TODO: base class this with the same game mode registering technique in SO_InputManager and PSOs

//Overlay and callback tutorial: https://steamworks.github.io/gettingstarted/#getting-started-with-steamworksnet

[CreateAssetMenu(fileName = "SO_SteamManager", menuName = "ScriptableObjects/StaticData/SO_SteamManager")]
public class SO_SteamManager : ScriptableObject
{
    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;



    [NonSerialized]
    private bool registeredWithGameModeManager = false;

    //Assigning callback prevents it from being garbage collected
    private Callback<GameOverlayActivated_t> gameOverlayActivated;

    private event Action<bool> onMenuOverlay = delegate { };

    private List<Action<bool>> nonPersistentSubscribers = new List<Action<bool>>();

    public bool TryCompleteAchievement(SteamAchievementsEnum sae)
    {
        if (sae == SteamAchievementsEnum.NONE) { return false; }
        if (!SteamManager.Initialized) { return false; }

        bool achieved;
        SteamUserStats.GetAchievement(sae.ToString(), out achieved);
        Debug.Log("Status of achievement: "+ sae.ToString() + " = " + achieved);
        SteamUserStats.SetAchievement(sae.ToString());
        SteamUserStats.StoreStats();
        return true;
    }

    public void RegisterForOnSteamOveraly(Action<bool> subscriber, bool persistent)
    {
        if (!registeredWithGameModeManager)
        {
            gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
            S_GameModeManager.Current.RegisterForGameModeSceneUnloaded(S_GameModeManager_OnGameModeSceneUnloaded);
            registeredWithGameModeManager = true;
        }

        onMenuOverlay -= subscriber;
        onMenuOverlay += subscriber;

        if (!persistent)
        {
            nonPersistentSubscribers.Add(subscriber);
        }
    }

    public void DeregisterFromOnSteamOverlay(Action<bool> subscriber)
    {
        onMenuOverlay -= subscriber;
        nonPersistentSubscribers.Remove(subscriber);
    }

    private void OnGameOverlayActivated(GameOverlayActivated_t ctx)
    {
        if (ctx.m_bActive != 0)
        {
            onMenuOverlay(true);
        }
        else
        {
            onMenuOverlay(false);
        }
    }
    

    private void S_GameModeManager_OnGameModeSceneUnloaded()
    {
        foreach(Action<bool> nps in nonPersistentSubscribers)
        {
            onMenuOverlay -= nps;
        }

        nonPersistentSubscribers.Clear();
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
