using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class BossSpawnBlackMargins : MonoBehaviour
{
    [SerializeField]
    private RectTransform topMarginBar = null;

    [SerializeField]
    private RectTransform bottomMarginBar = null;

    [SerializeField]
    private PSO_CurrentZonePhase currZonePhase = null;

    [SerializeField]
    private DSO_TerrainChange terrainChangeDelegate = null;

    [SerializeField]
    private float transitionTime = 1f;

    private Coroutine moveCR = null;

    private void Start()
    {
        currZonePhase.RegisterForPropertyChanged(OnPhaseChange);

        topMarginBar.gameObject.SetActive(true);
        bottomMarginBar.gameObject.SetActive(true);

        topMarginBar.localScale = new Vector3(1, 0, 1);
        bottomMarginBar.localScale = new Vector3(1, 0, 1);
    }

    private void OnPhaseChange(ZonePhaseEnum _, ZonePhaseEnum newPhase)
    {
        if (newPhase == ZonePhaseEnum.BOSS_SPAWN)
        {
            terrainChangeDelegate.RegisterForDelegateInvoked(OnTerrainChange);
        }

        if (newPhase == ZonePhaseEnum.BOSS)
        {
            SafeStartCoroutine(ref moveCR, MoveMargins(false), this);
        }
    }

    private int OnTerrainChange(TerrainChangeWrapper tcw)
    {
        terrainChangeDelegate.DeRegisterFromDelegateInvoked(OnTerrainChange);
        SafeStartCoroutine(ref moveCR, MoveMargins(true), this);
        return 0;
    }

    private IEnumerator MoveMargins(bool moveInOrOut)
    {
        //in case we change from move in to out quickly
        float startingScale = topMarginBar.localScale.y;
        float endScale = moveInOrOut ? 1 : 0;
        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / transitionTime;
            float yScale = Mathf.Lerp(startingScale, endScale, EasedPercent(perc));
            topMarginBar.localScale = new Vector3(1, yScale, 1);
            bottomMarginBar.localScale = new Vector3(1, yScale, 1);

            yield return null;
        }

        moveCR = null;
    }
}
