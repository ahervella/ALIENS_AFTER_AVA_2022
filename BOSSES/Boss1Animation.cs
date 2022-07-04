using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossOscillationManager))]
public class Boss1Animation : BaseAnimation<Boss1State, SO_Boss1AnimationSettings>
{
    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    private BossOscillationManager oscillationManager;


    protected override void OnActionChange(Boss1State prevAction, Boss1State newAction)
    {
        AnimationClip animClip = settings.GetAnimation(newAction);
        spriteAnimator.Play(animClip);

        if (newAction == Boss1State.IDLE)
        {
            oscillationManager.RestartOscillationAxis(xOrYAxis: true, false);
            oscillationManager.RestartOscillationAxis(xOrYAxis: false, false);
        }

        else
        {
            oscillationManager.ReduceOscillationAxis(xOrYAxis: true, 0.2f);
            oscillationManager.ReduceOscillationAxis(xOrYAxis: false, 0.2f);
        }
    }

    protected override void OnStart()
    {
        oscillationManager = GetComponent<BossOscillationManager>();
        oscillationManager.InitOscillation(
            settings.XOscillationTime,
            settings.XOscillationTileAmnt * terrSettings.TileDims.x,
            settings.YOscillationTime,
            settings.YOscillationTileAmnt * terrSettings.TileDims.y);
    }
}
