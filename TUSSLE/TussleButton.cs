using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PowerTools;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TussleButton : MonoBehaviour
{
    [SerializeField]
    private SO_TussleSettings settings = null;

    [SerializeField]
    private TextMeshProUGUI textMesh = null;

    [SerializeField]
    private AnimationEventExtender animEventExtender = null;
    [SerializeField]
    private SpriteAnim anim = null;
    [SerializeField]
    private Image animRenderer;

    [SerializeField]
    private List<ButtonSpriteWrapper> buttonSpriteWrappers = new List<ButtonSpriteWrapper>();

    [Serializable]
    private class ButtonSpriteWrapper
    {
        [SerializeField]
        private TussleButtonStateEnum buttonState;
        public TussleButtonStateEnum ButtonState => buttonState;

        [SerializeField]
        private Sprite buttonSprite = null;
        public Sprite ButtonSprite => buttonSprite;

        [SerializeField]
        private bool isUpPosition = false;
        public bool IsUpPosition => isUpPosition;

        [SerializeField]
        private Color buttonColor = Color.white;
        public Color ButtonColor => buttonColor;
    }

    [SerializeField]
    private AnimationClip pressNowAnim = null;

    [SerializeField]
    private Vector3 pressedOffset = default;

    private Vector3 textOriginalPos;

    private InputEnum assignedInput;
    public InputEnum AssignedInput => assignedInput;

    private TussleButtonStateEnum currState;

    private void Awake()
    {
        SetRandomRequiredInput();
        animEventExtender.AssignAnimationEvent(ExtendedAE_ButtonUp, 0);
        animEventExtender.AssignAnimationEvent(ExtendedAE_ButtonDown, 1);

        textOriginalPos = textMesh.rectTransform.anchoredPosition3D;

        SetButtonState(TussleButtonStateEnum.IDLE);
    }

    private void SetRandomRequiredInput()
    {
        ButtonCharacterWrapper bcw = settings.GetRandomCharacterWrapper();
        textMesh.text = bcw.Characters;
        assignedInput = bcw.Input;
    }

    public void SetButtonState(TussleButtonStateEnum state)
    {
        foreach(ButtonSpriteWrapper bsw in buttonSpriteWrappers)
        {
            if (bsw.ButtonState == state)
            {
                animRenderer.sprite = bsw.ButtonSprite;
                animRenderer.color = bsw.ButtonColor;
                textMesh.color = bsw.ButtonColor;

                if (state == TussleButtonStateEnum.PRESS_NOW)
                {
                    anim.Play(pressNowAnim);
                    return;
                }

                anim.Stop();

                if (bsw.IsUpPosition)
                {
                    ExtendedAE_ButtonUp();
                }
                else
                {
                    ExtendedAE_ButtonDown();
                }
                return;
            }
        }
        Debug.LogError($"Could not find sprite for button state {state}");
    }

    private void ExtendedAE_ButtonUp()
    {
        textMesh.rectTransform.anchoredPosition3D = textOriginalPos;
    }

    private void ExtendedAE_ButtonDown()
    {
        textMesh.rectTransform.anchoredPosition3D = textOriginalPos + pressedOffset;
    }

    public void SetToFailed()
    {
        if (currState == TussleButtonStateEnum.PRESS_NOW)
        {
            SetButtonState(TussleButtonStateEnum.FAILED_UNPRESSED);
        }

        SetButtonState(TussleButtonStateEnum.FAILED_PRESSED);
    }
}


