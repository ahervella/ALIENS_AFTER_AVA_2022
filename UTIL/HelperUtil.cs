using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

public static class HelperUtil
{
    /// <summary>
    /// Shortcut for adding a vector 3 to a transform's position
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
    /// Shortcut for adding a vector 3 to a transform's local position
    /// </summary>
    /// <param name="t"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void LocalPositionChange(Transform t, float x, float y, float z)
    {
        t.localPosition = new Vector3(t.localPosition.x + x, t.localPosition.y + y, t.localPosition.z + z);
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

        float theta = (2 * origPerc - 1 ) * Mathf.PI / 2f;
        return (Mathf.Sin(theta) + 1) / 2f;
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

    /// <summary>
    /// Set the hit box dimensions using the terr settings
    /// </summary>
    public static void SetHitBoxDimensions(
        BoxColliderSP hitBox,
        Vector2 objTileDims,
        int height,
        SO_TerrSettings terrSettings,
        Vector3PropertySO hitBoxDimEdgePercents,
        bool setHitBoxDefaultLocalPos = true)
    {
        SetHitBoxDimensionsAndPos(hitBox.Box(), objTileDims, height, terrSettings, hitBoxDimEdgePercents, setHitBoxDefaultLocalPos);
    }

    /// <summary>
    /// Set the hit box dimensions using the terr settings
    /// </summary>
    public static void SetHitBoxDimensionsAndPos(
        BoxCollider hitBox,
        Vector2 objTileDims,
        int height,
        SO_TerrSettings terrSettings,
        Vector3PropertySO hitBoxDimEdgePercents,
        bool setHitBoxDefaultLocalPos = true)
    {
        Vector3 hitBoxDimensions = new Vector3(
            objTileDims.x * terrSettings.TileDims.x,
            height * terrSettings.FloorHeight,
            objTileDims.y * terrSettings.TileDims.y);

        hitBoxDimensions -= new Vector3(
            terrSettings.TileDims.x * (1 - hitBoxDimEdgePercents.Value.x),
            terrSettings.FloorHeight * (1 - hitBoxDimEdgePercents.Value.y),
            terrSettings.TileDims.y * (1 - hitBoxDimEdgePercents.Value.z));

        hitBox.size = hitBoxDimensions;
        hitBox.center = new Vector3(0, hitBoxDimensions.y / 2f, 0);
        if (setHitBoxDefaultLocalPos)
        {
            hitBox.transform.localPosition = Vector3.zero;
        }
    }

    //TODO: do we even need this? Don't think it's currently used...
    /// <summary>
    /// Set the Reward Box Dimensions from scratch
    /// </summary>
    /// <param name="hitBoxSP"></param>
    /// <param name="energyRewardBox"></param>
    /// <param name="objTileDims"></param>
    /// <param name="height"></param>
    /// <param name="terrSettings"></param>
    /// <param name="hitBoxDimEdgePercents"></param>
    public static void SetRewardBoxDimensions(
        BoxColliderSP hitBoxSP,
        BoxColliderSP energyRewardBox,
        Vector2 objTileDims,
        int height,
        SO_TerrSettings terrSettings,
        Vector3PropertySO hitBoxDimEdgePercents
        )
    {
        SetHitBoxDimensions(
            hitBoxSP, objTileDims, height, terrSettings, hitBoxDimEdgePercents);

        SetRewardBoxDimensionsFromHB(hitBoxSP.Box(), energyRewardBox, terrSettings);
    }

    /// <summary>
    /// Set the Reward Box Dimensions from the object hit box that has
    /// already been set
    /// </summary>
    /// <param name="hitBox"></param>
    /// <param name="energyRewardBox"></param>
    /// <param name="terrSettings"></param>
    public static void SetRewardBoxDimensionsFromHB(
        BoxCollider hitBox,
        BoxColliderSP energyRewardBox,
        SO_TerrSettings terrSettings
        )
    {
        //Hack: save computation by using hit box values
        Vector3 rewardBoxDims = hitBox.size + new Vector3(0, 0, terrSettings.RewardBoxLengthFront);

        energyRewardBox.Box().size = rewardBoxDims;
        energyRewardBox.Box().center = new Vector3(0, rewardBoxDims.y / 2f, -(rewardBoxDims.z - hitBox.size.z) / 2f);
    }


    //TODO: convert all cr uses to use this

    /// <summary>
    /// Safely start a coroutine by checking to see if it its reference is running (aka not null).
    /// If so, stop that coroutine before beginning a new one
    /// </summary>
    /// <param name="cr">The coroutine reference</param>
    /// <param name="crMethod">The routine method</param>
    /// <param name="mbRef">MonoBeahviour reference to call Unity CR methods</param>
    public static void SafeStartCoroutine(ref Coroutine cr, IEnumerator crMethod, MonoBehaviour mbRef)
    {
        SafeStopCoroutine(ref cr, mbRef);
        cr = mbRef.StartCoroutine(crMethod);
    }

    /// <summary>
    /// Safely stops a coroutine by checking if its running first
    /// </summary>
    /// <param name="cr">The coroutine reference</param>
    /// <param name="mbRef">MonoBeahviour reference to call the Unity StopCR method</param>
    public static void SafeStopCoroutine(ref Coroutine cr, MonoBehaviour mbRef)
    {
        if (cr != null)
        {
            mbRef.StopCoroutine(cr);
        }
    }

    public static T InstantiateAndSetPosition<T>(T prefab, Transform prefabParent, Vector3 prefabPos) where T : MonoBehaviour
    {
        T instance = GameObject.Instantiate(prefab, prefabParent);
        instance.transform.position = prefabPos;
        return instance;
    }

    public static int GetLaneIndexFromPosition(float xPos, SO_TerrSettings terrSettings)
    {
        int laneIndexFromLeft = (int)Mathf.Floor(xPos / terrSettings.TileDims.x);
        return laneIndexFromLeft - terrSettings.LaneCount / 2;
    }

    public static int GetFloorIndexFromPosition(float yPos, SO_TerrSettings terrSettings)
    {
        return (int)Mathf.Floor(yPos / terrSettings.FloorHeight);
    }

    public static float GetLaneXPosition(int laneIndexFromCenter, SO_TerrSettings terrSettings)
    {
        return (terrSettings.LaneCount / 2 + laneIndexFromCenter + 0.5f) * terrSettings.TileDims.x;
    }

    public static float GetFloorYPosition(int floorIndex, SO_TerrSettings terrSettings)
    {
        return floorIndex * terrSettings.FloorHeight;
    }


    //TODO base class bosses with terr hazards so we don't need to share
    //code this way lol
    public static void MakeCustomHitBoxes(
        BoxColliderSP hitBox,
        Vector2Int hazardDims,
        //int hitBoxHeight,
        SO_TerrSettings terrSettings,
        Vector3PropertySO hitBoxDimEdgePercents,
        List<HitBoxWrapper> customHitBoxes)
    {
        foreach (HitBoxWrapper hbw in customHitBoxes)
        {
            BoxColliderSP instance = Object.Instantiate(hitBox, hitBox.transform.parent);
            int width = hbw.MaxXRange - hbw.MinXRange;
            Vector2Int dims = new Vector2Int(width, hazardDims.y);

            SetHitBoxDimensions(instance, dims, hbw.FloorHeight, terrSettings, hitBoxDimEdgePercents);

            float centerOffset = (width - hazardDims.x) / 2f + hbw.MinXRange;
            centerOffset *= terrSettings.TileDims.x;
            instance.Box().center = new Vector3(centerOffset, instance.Box().center.y, instance.Box().center.z);

            hbw.CacheInstancedHB(instance);
            instance.SetReqAvoidAction(hbw.CustomAvoidAction);
        }

        //disable original so we don't overlap with that
        hitBox.gameObject.SetActive(false);
    }

    //TODO: does this need to live here?
    public static void ApplyHitBoxSizeErrorFix(BoxColliderSP hb)
    {
        BoxCollider box = hb.Box();

        //TODO: realized that I set the standard of tiles and hit box length
        //with half of it behind the terr object. Cleaner way to fix?
        //Originally had to do this because the grapple was colliding with the back of the hazard
        box.size = new Vector3(box.size.x, box.size.y, box.size.z / 2f);
        box.center = new Vector3(box.center.x, box.center.y, -box.size.z / 2f);
    }

    //TODO: convert all places where unscaled time is being because of
    //tussles with this
    public static float UnscaledTimeIfNotPaused(bool paused)
    {
        if (paused)
        {
            return 0;
        }

        return Time.unscaledDeltaTime;
    }
}
