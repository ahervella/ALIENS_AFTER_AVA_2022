using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using PowerTools;

[RequireComponent(typeof(FaceCamera))]
[RequireComponent(typeof(SafeAudioWrapperSource))]
public class ElectronShield : MonoBehaviour
{
    [SerializeField]
    private BoolPropertySO shieldOnFlag = null;

    [SerializeField]
    private float shieldTime;

    [SerializeField]
    private DestructionSprite destructionSpritePrefab = null;

    [SerializeField]
    private SpriteAnim sprite = null;

    [SerializeField]
    private AnimationClip endAnim = null;

    [SerializeField]
    private float timeBeforeEndAnim = 1f;

    private Coroutine timerCR = null;

    private void Awake()
    {
        shieldOnFlag.ModifyValue(true);
        shieldOnFlag.RegisterForPropertyChanged(OnShieldFlagChange);
        timerCR = StartCoroutine(ShieldCoroutine());
    }

    private void OnShieldFlagChange(bool oldVal, bool newVal)
    {
        if (!newVal)
        {
            SafeDestroy(gameObject);
            shieldOnFlag.DeRegisterForPropertyChanged(OnShieldFlagChange);
        }
    }

    private IEnumerator ShieldCoroutine()
    {
        yield return new WaitForSeconds(shieldTime - timeBeforeEndAnim);
        sprite.Play(endAnim);
        sprite.SetTime(0);
        yield return new WaitForSeconds(timeBeforeEndAnim);
        shieldOnFlag.ModifyValue(false);
    }

    private void OnDestroy()
    {
        if (timerCR != null)
        {
            StopCoroutine(timerCR);
        }
        destructionSpritePrefab?.InstantiateDestruction(transform.position, transform.rotation);
    }
}
