using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PowerTools;

[RequireComponent(typeof(SpriteAnim))]
public class AnimationEventExtender : MonoBehaviour
{
    private Action[] methods = new Action[4];

    public void AssignAnimationEvent(Action method, int methodIndex)
    {
        if (methodIndex >= methods.Length)
        {
            Debug.LogError($"Not enough methods exist in AnimationEventExtender for index {methodIndex}");
            return;
        }

        methods[methodIndex] = method;
    }

    public void AE_Extender00()
    {
        methods[0]?.Invoke();
    }

    public void AE_Extender01()
    {
        methods[1]?.Invoke();
    }

    public void AE_Extender02()
    {
        methods[2]?.Invoke();
    }

    public void AE_Extender03()
    {
        methods[3]?.Invoke();
    }
}
