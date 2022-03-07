using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;
using UnityEngine.SceneManagement;

public class TussleManager : MonoBehaviour
{
    [SerializeField]
    private SpriteAnim tussleAnim = null;

    [SerializeField]
    private DSO_TreadmillSpeedChange treadmillSpeedDelegate = null;

    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;

    private void Awake()
    {
        //do tussle...
        //currAction.ModifyValue(PlayerActionEnum.TUSSLE);
        treadmillSpeedDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(0, 0));
    }

    public void InitiateTussle(bool playerAdvantage)
    {
        StartCoroutine(tempTussleCoroutine());
    }

    private IEnumerator tempTussleCoroutine()
    {
        yield return new WaitForSeconds(2);
        EndTussle();
    }

    private void EndTussle()
    {
        treadmillSpeedDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(1, 0));
        currAction.ModifyValue(PlayerActionEnum.RUN);
        Destroy(gameObject);
    }
}
