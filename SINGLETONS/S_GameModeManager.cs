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

    public void ReplaceGameModeScene(GameModeEnum gameMode, Action onFinishLoading = null)
    {
        if (loadingCR != null)
        {
            Debug.Log("Currently loading a scene already, can't load scene right now!");
            return;
        }

        OnGameModeUnloadedDelegate?.Invoke();

        AsyncLoadGameMode(gameMode, false, onFinishLoading);
    }

    private void AsyncLoadGameMode(GameModeEnum gameMode, bool additive, Action onFinishLoading = null)
    {
        string gameModeSceneName = settings.GetSceneName(gameMode);

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
