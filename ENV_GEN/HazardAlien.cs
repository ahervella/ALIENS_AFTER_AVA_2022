using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioWrapperSource))]
public class HazardAlien : TerrHazard
{
    [SerializeField]
    private AAudioWrapperV2 attackAudio = null;

    [SerializeField]
    private AnimationClip stunAnimation = null;

    [SerializeField]
    private AnimationClip attackAnimation = null;

    [SerializeField]
    private PlayerActionEnum takeDownReqAction = PlayerActionEnum.NONE;

    [SerializeField]
    private float timeBeforeTriggerAttack = 1f;

    [SerializeField]
    private FloatPropertySO currTreadmillSpeed = null;

    [SerializeField]
    public BoxColliderSP attackTrigger = null;

    protected bool stunnedFlag = false;

    protected override void Awake()
    {
        base.Awake();

        hazardTakeDownReqAction = takeDownReqAction;

        currTreadmillSpeed.RegisterForPropertyChanged(OnTreadmillSpeedChange);
        OnTreadmillSpeedChange(currTreadmillSpeed.Value, currTreadmillSpeed.Value);
        attackTrigger.SetOnTriggerMethod(OnTriggerEnterAttackBox);
    }

    private void OnTreadmillSpeedChange(float oldSpeed, float newSpeed)
    {
        float triggerBoxDepth = newSpeed * timeBeforeTriggerAttack;
        attackTrigger.Box().size = new Vector3(
            //plus 2 here so that animation can start if player is in an adjacent
            //lane of this alien
            (Dimensions().x + 2) * terrSettings.TileDims.x,
            terrSettings.FloorHeight,
            triggerBoxDepth);

        attackTrigger.Box().center = new Vector3(0, attackTrigger.Box().size.y / 2f, attackTrigger.Box().size.z / 2f);
    }

    private void OnTriggerEnterAttackBox(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerRunner>() != null)
        {
            if (!stunnedFlag)
            {
                sprite.Play(attackAnimation);
                attackAudio.PlayAudioWrapper(audioSource);
            }
            attackTrigger.SetOnTriggerMethod(null);
        }
    }

    public virtual void Stun()
    {
        stunnedFlag = true;
        hazardTakeDownReqAction = PlayerActionEnum.ANY_ACTION;
        sprite.Play(stunAnimation);
    }

    private void OnDestroy()
    {
        currTreadmillSpeed.DeRegisterForPropertyChanged(OnTreadmillSpeedChange);
    }
}
