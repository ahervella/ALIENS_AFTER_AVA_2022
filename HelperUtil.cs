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
        float theta = origPerc * Mathf.PI / 2f;
        float result = Mathf.Sin(theta);
        return result < 0.0001 ? 0 : (result > 0.9999? 1 : result);
    }
}
