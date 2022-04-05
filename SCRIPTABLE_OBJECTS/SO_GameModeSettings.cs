using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using static HelperUtil;

[CreateAssetMenu(fileName = "SO_GameModeSettings", menuName = "ScriptableObjects/StaticData/SO_GameModeSettings")]
public class SO_GameModeSettings : ScriptableObject
{
    [SerializeField]
    private List<GameModeWrapper> wrappers = null;

    [Serializable]
    private class GameModeWrapper
    {
        [SerializeField]
        private GameModeEnum gameMode = default;
        public GameModeEnum GameMode => gameMode;

        [SerializeField]
        private string sceneName = default;
        public string SceneName => sceneName;
    }

    public string GetSceneName(GameModeEnum gameMode)
    {
        GameModeWrapper wrapper = GetWrapperFromFunc(wrappers, gmw => gmw.GameMode, gameMode, LogEnum.WARNING, null);

        if (wrapper != null)
        {
            return wrapper.SceneName;
        }

        return string.Empty;
    }
}
