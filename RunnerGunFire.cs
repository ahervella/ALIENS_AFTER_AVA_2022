using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RunnerGameObject;

public class RunnerGunFire : MonoBehaviour
{
    public static event System.Action onGunFireAnimEnd = delegate { };
    public static event System.Action<RunnerThrowable.THROW_TYPE> onGunFired = delegate { };
    [SerializeField]
    private GameObject runnerObject = null;
    //used by RA_FIRE animation clip events
    void gunFireAnimEnded() => onGunFireAnimEnd();
    void gunFired() => onGunFired(RunnerThrowable.THROW_TYPE.BULLET);
    void playGunSound(AAudioWrapper aw) => RunnerSounds.Current.PlayAudioWrapper(aw, runnerObject);
}
