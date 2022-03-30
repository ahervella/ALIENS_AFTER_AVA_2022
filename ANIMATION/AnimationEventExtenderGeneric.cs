using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PowerTools;

[RequireComponent(typeof(SpriteAnim))]
public class AnimationEventExtender<T> : AAnimationEventExtender
{
    private readonly Action<T>[] objectMethods = new Action<T>[4];

    public void AssignAnimationEvent(Action<T> method, int methodIndex)
    {
        if (methodIndex >= objectMethods.Length)
        {
            IndexTooLargeDebugMessage(methodIndex);
            return;
        }

        if (objectMethods[methodIndex] != null)
        {
            IndexIsTakeDebugMessage(methodIndex);
            return;
        }

        objectMethods[methodIndex] = method;
    }


    public void AE_Extender00(T obj)
    {
        objectMethods[0]?.Invoke(obj);
    }

    public void AE_Extender01(T obj)
    {
        objectMethods[1]?.Invoke(obj);
    }

    public void AE_Extender02(T obj)
    {
        objectMethods[2]?.Invoke(obj);
    }

    public void AE_Extender03(T obj)
    {
        objectMethods[3]?.Invoke(obj);
    }
}
