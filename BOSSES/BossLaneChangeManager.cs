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

    private Coroutine laneChangeCR = null;

    public int CurrLane { private set; get; } = 0;

    public bool EnableAutoLaneChanging { set; get; } = true;

    private void Awake()
    {
        laneChangeDSO.RegisterForDelegateInvoked(OnLaneChangeDelegate);
    }

    private int OnLaneChangeDelegate(LaneChange lc)
    {
        if (!EnableAutoLaneChanging) { return 0; }
        if (Mathf.Abs(CurrLane - lc.Dir) > maxLaneDeviation) { return 0; }
        SafeStartCoroutine(ref laneChangeCR, ChangeLane(lc.Dir, lc.Time), this);
        return 0;
    }

    private IEnumerator ChangeLane(int dirMag, float time)
    {
        yield return new WaitForSeconds(laneChangeDelay);

        CurrLane -= dirMag;

        float perc = 0;
        float startXPos = transform.localPosition.x;
        float endXPos = GetLaneXPosition(CurrLane, terrSettings);

        while (perc < 1)
        {
            perc += Time.deltaTime / time;
            float newLocalXPos = Mathf.Lerp(startXPos, endXPos, EasedPercent(perc));
            targetTrans.localPosition = new Vector3(
                newLocalXPos,
                targetTrans.localPosition.y,
                targetTrans.localPosition.z);
            yield return null;
        }
    }

    public void MoveToLane(int laneIndex)
    {
        if (Mathf.Abs(laneIndex) > maxLaneDeviation) { return; }
        SafeStartCoroutine(ref laneChangeCR,ChangeLane(
            CurrLane - laneIndex,
            manualLaneChangeTime),
            this);
    }
}
