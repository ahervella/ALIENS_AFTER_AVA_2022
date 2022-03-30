using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

public static class HelperUtil
{
    /// <summary>
    /// Shortcut for adding a vector 3 to a transforms position
    /// </summary>
    /// <param name="t"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void PositionChange(Transform t, float x, float y, float z)
    {
        t.position = new Vector3(t.position.x + x, t.position.y + y, t.position.z + z);
    }

    /// <summary>
    /// An easer for the percent of a lerp to smooth into and out of the transition
    /// </summary>
    /// <param name="origPerc"></param>
    /// <returns></returns>
    public static float EasedPercent(float origPerc)
    {
        //TODO: seems to solve bug, but is this safe to do and can we always expect to get
        //a perfect 0 or 1 float value from the math here?
        origPerc = Mathf.Clamp(origPerc, 0, 1);
        float theta = origPerc * Mathf.PI / 2f;
        return Mathf.Sin(theta);
    }

    /// <summary>
    /// Safely destroys a gameobject
    /// </summary>
    /// <param name="go">GameObject to destroy</param>
    public static void SafeDestroy(GameObject go)
    {
        SafeAudioWrapperSource saws = go.GetComponent<SafeAudioWrapperSource>();
        if (saws == null)
        {
            Object.Destroy(go);
            return;
        }

        saws.SafeDestroy();
    }

    /// <summary>
    /// Helper function that gets a wrapper from a list of wrappers given a key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <param name="wrappers">List of wrappers to search</param>
    /// <param name="wrapper2Key">Function for getting the wrapper's key</param>
    /// <param name="targetKey">Key we are looking to match</param>
    /// <param name="debugLogType">Type of log to print</param>
    /// <param name="failReturnVal">Fail return value</param>
    /// <param name="customMsg">Custom log message</param>
    /// <returns>The wrapper that has the matching target key</returns>
    public static T GetWrapperFromFunc<T, K>(
        List<T> wrappers,
        Func<T, K> wrapper2Key,
        K targetKey,
        LogEnum debugLogType,
        T failReturnVal,
        string customMsg = null)
    {
        foreach(T wrapper in wrappers)
        {
            if (targetKey.Equals(wrapper2Key(wrapper)))
            {
                return wrapper;
            }
        }

        string msg = customMsg ?? $"Could not find {targetKey} amongst {wrappers}";

        switch (debugLogType)
        {
            case LogEnum.WARNING:
                Debug.LogWarning(msg);
                break;
            case LogEnum.ERROR:
                Debug.LogError(msg);
                break;
        }

        return failReturnVal;
    }

    public enum LogEnum
    {
        NONE = 0, WARNING = 1, ERROR = 2
    }
}
