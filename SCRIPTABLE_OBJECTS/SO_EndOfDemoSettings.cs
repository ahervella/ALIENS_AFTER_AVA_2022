using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_EndOfDemoSettings", menuName = "ScriptableObjects/StaticData/SO_EndOfDemoSettings")]
public class SO_EndOfDemoSettings : ScriptableObject
{
    [SerializeField]
    private float fade2BlackTime = 1f;
    public float Fade2BlackTime => fade2BlackTime;

    [TextArea(2, 2)]
    [SerializeField]
    private string url = string.Empty;
    public string URL => url;

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
