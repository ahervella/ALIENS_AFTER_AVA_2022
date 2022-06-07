using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossOscillationManager))]
public class Boss3Animation : BaseAnimation<Boss3State, SO_Boss3AnimationSettings>
{
    [SerializeField]
    private SO_TerrSettings terrSettings = null;

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
        oscillationManager.InitOscillation(
            settings.XOscillationTime,
            settings.XOscillationTileAmnt * terrSettings.TileDims.x,
            settings.YOscillationTime,
            settings.YOscillationTileAmnt * terrSettings.TileDims.y);
    }
}
