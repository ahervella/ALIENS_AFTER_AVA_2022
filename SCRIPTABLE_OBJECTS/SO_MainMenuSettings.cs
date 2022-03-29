using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_MainMenuSettings", menuName = "ScriptableObjects/StaticData/SO_MainMenuSettings")]
public class SO_MainMenuSettings : ScriptableObject
{

    [SerializeField]
    private float videoDelayFromBlack = default;
    public float VideoDelayFromBlack => videoDelayFromBlack;

    [SerializeField]
    private float videoFadeInTime = default;
    public float VideoFadeInTime => videoFadeInTime;

    [SerializeField]
    private float titleDelayFromVideoEnd = default;
    public float TitleTotalDelay => videoDelayFromBlack + videoFadeInTime + titleDelayFromVideoEnd;

    [SerializeField]
    private float titleFadeInTime = default;
    public float TitleFadeInTime => titleFadeInTime;

    [SerializeField]
    private float buttonPromptDelayFromTitle = default;
    public float ButtonPromptTotalDelay => TitleTotalDelay + titleFadeInTime + buttonPromptDelayFromTitle;

    [SerializeField]
    private float buttonPromptFadeInTime = default;
    public float ButtonPromptFadeInTime => buttonPromptFadeInTime;
}
