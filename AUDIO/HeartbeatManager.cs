using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatManager : MonoBehaviour
{
    [SerializeField]
    private SO_HeartbeatSettings settings = null;

    [SerializeField]
    private IntPropertySO currLives = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    private Coroutine heartbeatCR = null;
    private float currDelay;

    private void Awake()
    {
        //All will trigger once play scene loaded
        currLives.RegisterForPropertyChanged(OnLivesChanged);
        currGameMode.RegisterForPropertyChanged(OnGameModeChanged);
        OnLivesChanged(-1, currLives.Value);
        StartHeartbeat();
    }

    private void OnLivesChanged(int oldLives, int newLives)
    {
        foreach(HeartbeatDelayWrapper hbdw in settings.HeartbeatDelays)
        {
            if (newLives == hbdw.LivesLeft)
            {
                currDelay = hbdw.LivesLeft;
                return;
            }
        }
        Debug.LogError($"No heartbeat delay setting found for {newLives} lives :(");
    }

    private void OnGameModeChanged(GameModeEnum oldMode, GameModeEnum newMode)
    {
        if (newMode == GameModeEnum.PAUSE)
        {
            //it's fine if we just restart the coroutine from 0
            StopCoroutine(heartbeatCR);
        }

        if (newMode == GameModeEnum.PLAY || oldMode == GameModeEnum.PAUSE)
        {
            StartHeartbeat();
        }
    }

    private void StartHeartbeat()
    {
        if (heartbeatCR != null)
        {
            StopCoroutine(heartbeatCR);
        }
        heartbeatCR = StartCoroutine(HeartbeatLoop());
    }

    private IEnumerator HeartbeatLoop()
    {
        yield return new WaitForSeconds(currDelay);
        settings.PlayAllACWs(gameObject);
    }
}
