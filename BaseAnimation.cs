using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;

/// <summary>
/// Base class for all things that will use animation, and that
/// will have an enum (and a property SO) dictating their action.
/// Planning to use for player and bosses.
/// </summary>
/// <typeparam name="T"></typeparam>
[RequireComponent(typeof(SpriteAnim))]
public abstract class BaseAnimation<T> : MonoBehaviour
{
    [SerializeField]
    protected PropertySO<T> currAction = null;

    [SerializeField]
    protected SO_AnimationSettings<T> settings = null;

    protected SpriteAnim spriteAnimator;
    protected Animator animator;

    private void Awake()
    {
        currAction.RegisterForPropertyChanged(OnPlayerActionChange);
        spriteAnimator = GetComponent<SpriteAnim>();
        //included component if SpriteAnim is also included
        animator = GetComponent<Animator>();
    }

    protected abstract void OnPlayerActionChange(T prevAction, T newAction);

    public void AE_OnAnimFinished()
    {
        AninmationWrapper<T> aw = settings.GetAnimationWrapper(currAction.Value);
        if (aw != null)
        {
            currAction.ModifyValue(aw.ActionOnFinished);
            SetCurrentAnimationFrame(aw.NextActionStartFrameIndex);
        }
    }

    protected void SetCurrentAnimationFrame(int frame)
    {
        AnimationClip ac = spriteAnimator.GetCurrentAnimation();
        spriteAnimator.SetTime(frame / (ac.frameRate / ac.length));

        //force an animation update so this takes place immediately
        animator.Update(0.0f);
    }

    public void AE_TestFrameLand(string text)
    {
        Debug.LogFormat("Test frame land: {0}", text);
    }
}
