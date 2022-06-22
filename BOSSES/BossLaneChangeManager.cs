using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class BossLaneChangeManager : MonoBehaviour
{
    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    [SerializeField]
    private Transform targetTrans = null;

    [SerializeField]
    private DSO_LaneChange laneChangeDSO = null;

    [SerializeField]
    private int maxLaneDeviation = default;

    [SerializeField]
    private float laneChangeDelay = 0f;

    [SerializeField]
    private float manualLaneChangeTime = 1f;

    [SerializeField]
    private PSO_LaneChange optionalSelfLaneChangePSO = null;

    private Coroutine laneChangeCR = null;

    public int CurrLaneDeviation { private set; get; } = 0;

    public bool EnableAutoLaneChanging { set; get; } = true;

    private bool cachedInterruptableFlag = true;

    private void Awake()
    {
        laneChangeDSO.RegisterForDelegateInvoked(OnLaneChangeDelegate);
    }

    private int OnLaneChangeDelegate(LaneChange lc)
    {
        if (!EnableAutoLaneChanging) { return 0; }
        if (!cachedInterruptableFlag) { return 0; }
        if (Mathf.Abs(CurrLaneDeviation - lc.Dir) > maxLaneDeviation) { return 0; }
        SafeStartCoroutine(ref laneChangeCR, ChangeLane(lc.Dir, lc.Time), this);
        return 0;
    }

    private IEnumerator ChangeLane(int dirMag, float time)
    {
        yield return new WaitForSeconds(laneChangeDelay);

        CurrLaneDeviation -= dirMag;
        optionalSelfLaneChangePSO?.ModifyValue(new LaneChange(dirMag, time));

        float perc = 0;
        float startXPos = transform.position.x;
        float endXPos = GetLaneXPosition(CurrLaneDeviation, terrSettings);

        while (perc < 1)
        {
            perc += Time.deltaTime / time;
            float newXPos = Mathf.Lerp(startXPos, endXPos, EasedPercent(perc));
            targetTrans.position = new Vector3(
                newXPos,
                targetTrans.position.y,
                targetTrans.position.z);
            yield return null;
        }

        cachedInterruptableFlag = true;
    }

    public void MoveToLane(int laneIndex, bool interruptable = true)
    {
        if (Mathf.Abs(laneIndex) > maxLaneDeviation) { return; }
        if (!cachedInterruptableFlag) { return; }
        cachedInterruptableFlag = interruptable;

        SafeStartCoroutine(ref laneChangeCR,ChangeLane(
            CurrLaneDeviation - laneIndex,
            manualLaneChangeTime),
            this);
    }

    private void OnDestroy()
    {
        laneChangeDSO.DeRegisterFromDelegateInvoked(OnLaneChangeDelegate);
    }
}
