using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class MM_PauseMenuManager : A_MenuManager<PauseMenuButtonEnum>
{
    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private GameObject pauseMenuContainer = null;

    protected override void OnMenuAwake()
    {
        inputManager.RegisterForInput(InputEnum.GAME_PAUSE, InputManager_OnGamePause);

        AssignOnButtonPressedMethod(PauseMenuButtonEnum.RESUME, OnResume);
        AssignOnButtonPressedMethod(PauseMenuButtonEnum.QUIT, OnQuit);

        pauseMenuContainer.SetActive(false);
    }

    private void InputManager_OnGamePause(CallbackContext context)
    {
        if (currGameMode.Value == GameModeEnum.PAUSE)
        {
            OnResume();
            return;
        }

        OnPause();
    }

    private void OnPause()
    {
        Time.timeScale = 0;
        currGameMode.ModifyValue(GameModeEnum.PAUSE);
        pauseMenuContainer.SetActive(true);
    }

    private void OnResume()
    {
        Time.timeScale = 1;
        pauseMenuContainer.SetActive(false);
        currGameMode.ModifyValue(GameModeEnum.PLAY);
    }

    private void OnQuit()
    {
        currGameMode.ModifyValue(GameModeEnum.MAINMENU);
        Time.timeScale = 1;
    }

    protected override void OnMenuStart()
    {
    }
}

public enum PauseMenuButtonEnum
{
    RESUME = 0, QUIT = 1
}
