using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
