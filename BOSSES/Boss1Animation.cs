using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class Boss1Animation : BaseAnimation<Boss1State, SO_Boss1AnimationSettings>
{
    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    private class OscillationConfig
    {
        public float prevPos = 0;
        public float perc = 0;
        public float dir;
        public float delayOffset = 0;
        public float OscillationTime { get; private set; }
        public float targetTime;
        public float OscillationAmnt { get; private set; }
        public float min = 0;
        public float targetMin = 0;
        public float max = 0;
        public float targetMax = 0;
        public Vector3 TargetAxis { get; private set; }
        public Transform TargetTransform { get; private set; }

        public OscillationConfig(
            float OscillationTime,
            float OscillationAmnt,
            Vector3 TargetAxis,
            Transform TargetTransform)
        {
            this.OscillationTime = OscillationTime;
            this.OscillationAmnt = OscillationAmnt;
            this.TargetAxis = TargetAxis;
            this.TargetTransform = TargetTransform;

            dir = Random.value > 0.5 ? -1 : 1;
            targetTime = this.OscillationTime;
        }
    }

    OscillationConfig xConfig;
    OscillationConfig yConfig;


    protected override void OnActionChange(Boss1State prevAction, Boss1State newAction)
    {
        AnimationClip animClip = settings.GetAnimation(newAction);
        spriteAnimator.Play(animClip);

        if (newAction == Boss1State.IDLE)
        {
            RestartOscillationAxis(xConfig, false);
            RestartOscillationAxis(yConfig, false);
        }

        else
        {
            ReduceOscillationAxis(xConfig, 0.2f);
            ReduceOscillationAxis(yConfig, 0.2f);
        }
    }

    protected override void OnAwake()
    {
    }

    private void Start()
    {
        xConfig = new OscillationConfig(
            settings.XOscillationTime,
            settings.XOscillationTileAmnt * terrSettings.TileDims.x,
            new Vector3(1, 0, 0),
            transform);

        yConfig = new OscillationConfig(
            settings.YOscillationTime,
            settings.YOscillationTileAmnt * terrSettings.TileDims.y,
            new Vector3(0, 1, 0),
            transform);


        RestartOscillationAxis(xConfig, false);
        RestartOscillationAxis(yConfig, true);
    }

    private void RestartOscillationAxis(OscillationConfig oc, bool randOffset)
    {
        oc.targetMax = oc.OscillationAmnt;
        oc.targetMin = -oc.OscillationAmnt;
        oc.targetTime = oc.OscillationTime;
        oc.delayOffset = randOffset ? Random.value * oc.OscillationTime : 0;
    }

    private void ReduceOscillationAxis(OscillationConfig oc, float perc)
    {
        oc.targetMax = oc.OscillationAmnt * perc;
        oc.targetMin = -oc.OscillationAmnt * perc;
    }

    private void Update()
    {
        TickOscillation(xConfig);
        TickOscillation(yConfig);
    }

    private void TickOscillation(OscillationConfig oc)
    {
        if (oc.delayOffset > 0)
        {
            oc.delayOffset -= Time.deltaTime;
            return;
        }

        oc.perc += Time.deltaTime * oc.dir / oc.targetTime;

        float pos = Mathf.Lerp(oc.min, oc.max, EasedPercent(oc.perc));

        if (oc.perc >= 1)
        {
            oc.perc = 1;
            oc.dir = -1;
            oc.min = oc.targetMin;
            pos = oc.max;
        }
        else if(oc.perc <= 0)
        {
            oc.perc = 0;
            oc.dir = 1;
            oc.max = oc.targetMax;
            pos = oc.min;
        }

        oc.TargetTransform.localPosition += (pos - oc.prevPos) * oc.TargetAxis;
        oc.prevPos = pos;
    }
}
