using System.Collections;
using System.Collections.Generic;
using Tayx.Graphy;
using UnityEngine;
using UnityEngine.InputSystem;
using static HelperUtil;
using static UnityEngine.InputSystem.InputAction;

public class S_CursorManager : Singleton<S_CursorManager>
{
    [SerializeField]
    private float visibleTime = 0f;

    [SerializeField]
    private InputActionReference mouseMoveInput = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private BoolPropertySO devMenuVisible = null;

    private Vector2 lastPos;

    private Coroutine hideMouseCR = null;

    private bool visibilityOverride = false;

    protected override void OnAwake()
    {
        currGameMode.RegisterForPropertyChanged(OnGameModeChange);
        devMenuVisible.RegisterForPropertyChanged(OnDevMenuVisibleChange);
    }

    private void OnGameModeChange(GameModeEnum _, GameModeEnum newMode)
    {
        if (devMenuVisible.Value) { return; }

        visibilityOverride = true;

        switch (newMode)
        {
            case GameModeEnum.PLAY:
                Cursor.visible = false;
                break;

            case GameModeEnum.PAUSE:
                Cursor.visible = true;
                break;

            default:
                visibilityOverride = false;
                break;
        }
    }

    private void OnDevMenuVisibleChange(bool prevVisibility, bool newVisibility)
    {
        if (newVisibility)
        {
            Cursor.visible = true;
            visibilityOverride = true;
            return;
        }

        OnGameModeChange(currGameMode.Value, currGameMode.Value);
    }

    private void Update()
    {
        if (visibilityOverride) { return; }

        Vector2 newPos = mouseMoveInput.action.ReadValue<Vector2>();
        Debug.Log(newPos);
        if (newPos != lastPos)
        {
            Cursor.visible = true;
            SafeStartCoroutine(ref hideMouseCR, StartHideTimer(), this);
        }
        lastPos = newPos;
    }

    private IEnumerator StartHideTimer()
    {
        yield return new WaitForSeconds(visibleTime);
        Cursor.visible = false;
    }
}
