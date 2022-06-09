using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossOscillationManager))]
public class Boss3Animation : BaseAnimation<Boss3State, SO_Boss3AnimationSettings>
{
    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    [SerializeField]
    private List<BossOscillationManager> cannonDroneOscillators = new List<BossOscillationManager>();


    private BossOscillationManager oscillationManager;

    protected override void OnActionChange(Boss3State prevAction, Boss3State newAction)
    {
    }

    protected override void OnAwake()
    {
        oscillationManager = GetComponent<BossOscillationManager>();
    }

    private void Start()
    {
        StartOscillation(oscillationManager);

        foreach(BossOscillationManager bom in cannonDroneOscillators)
        {
            StartOscillation(bom);
        }
    }

    private void StartOscillation(BossOscillationManager bom)
    {
        bom.InitOscillation(
            settings.XOscillationTime,
            settings.XOscillationTileAmnt * terrSettings.TileDims.x,
            settings.YOscillationTime,
            settings.YOscillationTileAmnt * terrSettings.TileDims.y);
    }
}
