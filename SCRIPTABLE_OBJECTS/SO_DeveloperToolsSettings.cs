using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_DeveloperToolsSettings", menuName = "ScriptableObjects/StaticData/SO_DeveloperToolsSettings")]
public class SO_DeveloperToolsSettings : ScriptableObject
{
    [SerializeField]
    private bool invincibility = false;
    public bool Invincibility => invincibility;

    [SerializeField]
    private bool instantMainMenuIntro = false;
    public bool InstantMainMenuIntro => instantMainMenuIntro;

    //TODO: mute just music, infinite energy bar, no delay for changing moves, no timer delays, etc.
}
