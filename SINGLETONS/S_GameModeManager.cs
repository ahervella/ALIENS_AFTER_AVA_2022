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
    private Coroutine additiveLoadingCR = null;

    public void LoadGameMode(GameModeEnum gameMode, bool additive, Action onFinishLoading = null)
    {
        if (!additive && loadingCR != null)
        {
            Debug.Log("Currently loading a scene already, can't load scene right now!");
            return;
        }

        if (additive && additiveLoadingCR != null)
        {
            Debug.Log("Currently loading an additive scene already, can't load scene right now!");
            return;
        }

        string gameModeSceneName = settings.GetSceneName(gameMode);

        loadingCR = StartCoroutine(AsyncLoadGameSceneCR(gameModeSceneName, additive, onFinishLoading));
    }

    private IEnumerator AsyncLoadGameSceneCR(string sceneName, bool additive, Action onFinishLoading)
    {
        LoadSceneMode mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        onFinishLoading?.Invoke();
    }
}
