using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RunnerGameObject;

public class RunnerGunFire : MonoBehaviour
{
    public static event System.Action OnGunFireAnimEnd = delegate { };
    public static event System.Action<RunnerThrowable.THROW_TYPE> OnGunFired = delegate { };
    public static event System.Action<RunnerThrowable.THROW_TYPE> OnGunThrow = delegate { };

    //used by RA_FIRE animation clip events
    void gunFireAnimEnded() => OnGunFireAnimEnd();
    void gunFired() => OnGunFired(RunnerThrowable.THROW_TYPE.BULLET);
    void gunThrown() => OnGunThrow(RunnerThrowable.THROW_TYPE.GUN);
}
