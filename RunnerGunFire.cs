using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RunnerGameObject;

public class RunnerGunFire : MonoBehaviour
{
    public static event System.Action onGunFireAnimEnd = delegate { };
    public static event System.Action<RunnerThrowable.THROW_TYPE> onGunFired = delegate { };
    [SerializeField]
    private AAudioWrapper gunSound = null;

    //used by RA_FIRE animation clip events
    void gunFireAnimEnded() => onGunFireAnimEnd();
    void gunFired() => onGunFired(RunnerThrowable.THROW_TYPE.BULLET);
}
