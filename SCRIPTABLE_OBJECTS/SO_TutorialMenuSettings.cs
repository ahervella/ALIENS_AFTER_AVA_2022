using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_TutorialMenuSettings", menuName = "ScriptableObjects/StaticData/SO_TutorialMenuSettings")]
public class SO_TutorialMenuSettings : ScriptableObject
{
    [SerializeField]
    private float totalFadeTime = 1f;
    public float TotalFadeTime => totalFadeTime;

    [SerializeField]
    private List<string> lastWordsTexts = new List<string>();
    public string LastWordsText => lastWordsTexts[Random.Range(0, lastWordsTexts.Count)];

    [SerializeField]
    private float lastWordsDelay = 1f;
    public float LastWordsDelay => lastWordsDelay;

    [SerializeField]
    private float lastWordsTime = 1f;
    public float LastWordsTime => lastWordsTime;

    [SerializeField]
    private float lastWordsAfterDelay = 1f;
    public float LastWordsAfterDelay => lastWordsAfterDelay;
}
