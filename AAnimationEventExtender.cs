using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AAnimationEventExtender : MonoBehaviour
{
    protected void IndexTooLargeDebugMessage(int index)
    {
        Debug.LogError($"Not enough methods exist in AnimationEventExtender for index {index}");
    }

    protected void IndexIsTakeDebugMessage(int index)
    {
        Debug.LogError($"The index {index} is already taken!");
    }
}
