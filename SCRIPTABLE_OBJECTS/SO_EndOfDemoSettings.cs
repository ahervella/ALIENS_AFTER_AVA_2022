using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_EndOfDemoSettings", menuName = "ScriptableObjects/StaticData/SO_EndOfDemoSettings")]
public class SO_EndOfDemoSettings : ScriptableObject
{
    [SerializeField]
    private float fade2BlackTime = 1f;
    public float Fade2BlackTime => fade2BlackTime;

    [SerializeField]
    [TextArea(5, 5)]
    private string endOfDemoText = string.Empty;
    public string EndOfDemoText => endOfDemoText;

    [SerializeField]
    private string pressAnyInputText = string.Empty;
    public string PressAnyInputText => pressAnyInputText;

    [SerializeField]
    private float textDelay = 1f;
    public float TextDelay => textDelay;

    [SerializeField]
    private float textFadeInTime = 1f;
    public float TextFadeInTime => textFadeInTime;

    [SerializeField]
    private float showTextTime = 1f;
    public float ShowTextTime => showTextTime;

    [SerializeField]
    private float transition2MainMenuDelay = 1f;
    public float Transition2MainMenuDelay => transition2MainMenuDelay;
}
