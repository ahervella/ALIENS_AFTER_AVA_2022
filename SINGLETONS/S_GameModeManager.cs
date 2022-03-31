using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class S_GameModeManager : Singleton<S_GameModeManager>
{
    [SerializeField]
    private SO_GameModeSettings settings = null;

    private Coroutine loadingCR = null;
    //private Coroutine loadingAddedCR = null;

    //TODO: do we need to have the old and new GameModeEnum values? Can we live without?
    public delegate void GameModeUnloadedDelegate();

    private GameModeUnloadedDelegate OnGameModeUnloadedDelegate;

    //TODO: delete if we follow through with only relying on prefabs for additive operations
    /*
    public void AddGameModeScene(GameModeEnum gameMode, Action onFinishLoading = null)
    {
        if (loadingAddedCR != null)
        {
            Debug.Log("Currently loading an additive scene already, can't load scene right now!");
            return;
        }

        AsyncLoadGameMode(gameMode, true, onFinishLoading);
    }*/

    //TODO: should this be private and subscribe to the gamemode change (and/or do we want to make sure
    //all things get that change before changing scenes?)
    public bool TryReplaceGameModeScene(GameModeEnum gameMode, Action onFinishLoading = null)
    {
        if (loadingCR != null)
        {
            Debug.Log("Currently loading a scene already, can't load scene right now!");
            return false;
        }

        OnGameModeUnloadedDelegate?.Invoke();

        string gameModeSceneName = settings.GetSceneName(gameMode);

        if (gameModeSceneName.Equals(string.Empty))
        {
            Debug.Log("No scene found for game mode " + gameMode.ToString());
            return false;
        }

        AsyncLoadGameMode(gameModeSceneName, false, onFinishLoading);
        return true;
    }

    private void AsyncLoadGameMode(string gameModeSceneName, bool additive, Action onFinishLoading = null)
    {
        Action onFinishCR = delegate { };
        onFinishCR += onFinishLoading;

        if (!additive)
        {
            onFinishCR += () => loadingCR = null;
        }
        //else
        //{
        //    onFinishCR += () => loadingAddedCR = null;
        //}

        loadingCR = StartCoroutine(AsyncLoadGameModeSceneCR(gameModeSceneName, additive, onFinishCR));
    }

    private IEnumerator AsyncLoadGameModeSceneCR(string sceneName, bool additive, Action onFinishCR)
    {
        LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        onFinishCR?.Invoke();
    }

    public void RegisterForGameModeSceneUnloaded(GameModeUnloadedDelegate delegateMethod)
    {
        OnGameModeUnloadedDelegate -= delegateMethod;
        OnGameModeUnloadedDelegate += delegateMethod;
    }
}
