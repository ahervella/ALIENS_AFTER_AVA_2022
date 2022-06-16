using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

/// <summary>
/// Base class for all things that will use animation, and that
/// will have an enum (and a property SO) dictating their action.
/// Planning to use for player and bosses.
/// </summary>
/// <typeparam name="STATE"></typeparam>
[RequireComponent(typeof(SpriteAnim))]
[RequireComponent(typeof(AnimationEventExtender))]
public abstract class BaseAnimation<STATE, ANIM_SETTINGS> : MonoBehaviour where ANIM_SETTINGS : SO_AnimationSettings<STATE>
{
    [SerializeField]
    protected PropertySO<STATE> currAction = null;

    [SerializeField]
    protected ANIM_SETTINGS settings = null;

    protected SpriteAnim spriteAnimator;
    protected Animator animator;

    private void Awake()
    {
        currAction.RegisterForPropertyChanged(OnActionChange);
        spriteAnimator = GetComponent<SpriteAnim>();
        //included component if SpriteAnim is also included
        animator = GetComponent<Animator>();
        OnAwake();
    }

    protected abstract void OnAwake();

    protected abstract void OnActionChange(STATE prevAction, STATE newAction);

    public virtual void AE_OnAnimFinished()
    {
        AnimationWrapper<STATE> aw = settings.GetAnimationWrapper(currAction.Value);
        if (aw != null)
        {
            currAction.ModifyValue(aw.ActionOnFinished);
            SetCurrentAnimationFrame(aw.NextActionStartFrameIndex);
        }
    }

    protected void SetCurrentAnimationFrame(int frame)
    {
        AnimationClip ac = spriteAnimator.GetCurrentAnimation();
        spriteAnimator.SetTime(frame / (ac.frameRate * ac.length));

        //force an animation update so this takes place immediately
        animator.Update(0.0f);
    }

    public void AE_TestFrameLand(string text)
    {
        Debug.LogFormat("Test frame land: {0}", text);
    }
}
