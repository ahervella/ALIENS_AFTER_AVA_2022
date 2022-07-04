using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class S_GameModeManager : Singleton<S_GameModeManager>
{
    [SerializeField]
    private SO_GameModeSettings settings = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    private Coroutine loadingCR = null;
    //private Coroutine loadingAddedCR = null;

    //TODO: do we need to have the old and new GameModeEnum values? Can we live without?
    public delegate void GameModeUnloadedDelegate();

    private GameModeUnloadedDelegate OnGameModeUnloadedDelegate;

    private string currentSceneName = string.Empty;

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

    protected override void OnAwake()
    {
        //need to do this in awake because start runs every scene change,
        //we only need this once
        //Helpful to have this so we don't have to manually switch the GameMode PSO
        //we're working with in the editor
        StartCoroutine(StartCR());
    }
    
    private IEnumerator StartCR()
    {
        yield return null;
        currentSceneName = SceneManager.GetActiveScene().name;
        if (settings.GetSceneName(currGameMode.Value) != currentSceneName)
        {
            currGameMode.ModifyValue(settings.GetEnum(currentSceneName));
        }
    }

    //TODO: should this be private and subscribe to the gamemode change (and/or do we want to make sure
    //all things get that change before changing scenes?)
    public bool TryReplaceGameModeScene(GameModeEnum newMode, Action onFinishLoading = null)
    {
        if (loadingCR != null)
        {
            Debug.Log("Currently loading a scene already, can't load scene right now!");
            return false;
        }

        string gameModeSceneName = settings.GetSceneName(newMode);

        if (gameModeSceneName.Equals(string.Empty))
        {
            Debug.Log("No scene found for game mode " + newMode.ToString());
            return false;
        }

        if (currentSceneName.Equals(string.Empty))
        {
            //in case we started the game not from boot
            currentSceneName = SceneManager.GetActiveScene().name;
        }

        if (gameModeSceneName.Equals(currentSceneName))
        {
            //still in the same scene, no need to switch
            Debug.Log("Desired scene switch is current scene for game mode:" + newMode.ToString());
            return false;
        }

        currentSceneName = gameModeSceneName;

        AsyncLoadGameMode(gameModeSceneName, false, onFinishLoading);
        return true;
    }

    private void AsyncLoadGameMode(string gameModeSceneName, bool additive, Action onFinishLoading = null)
    {
        OnGameModeUnloadedDelegate?.Invoke();

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

    public void QuitGame()
    {
        OnGameModeUnloadedDelegate?.Invoke();
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
               Application.Quit();
        #endif
    }
}
