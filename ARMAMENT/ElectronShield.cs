using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

[RequireComponent(typeof(FaceCamera))]
public class ElectronShield : MonoBehaviour
{
    [SerializeField]
    private BoolPropertySO shieldOnFlag = null;

    [SerializeField]
    private float shieldTime;

    [SerializeField]
    private DestructionSprite destructionSpritePrefab = null;

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
        yield return new WaitForSeconds(shieldTime);
        shieldOnFlag.ModifyValue(false);
    }

    private void OnDestroy()
    {
        if (timerCR != null)
        {
            StopCoroutine(timerCR);
        }
        destructionSpritePrefab?.InstantiateDestruction(transform.parent, transform);
    }
}
