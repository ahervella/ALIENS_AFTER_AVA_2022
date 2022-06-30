using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class ArmamentAvailableManager : MonoBehaviour
{
    [SerializeField]
    private IntPropertySO armamentEnergyReq = null;

    [SerializeField]
    private PSO_CurrentEnergy currEnergy = null;

    [SerializeField]
    private PSO_FillBarQuant currTimer = null;

    [SerializeField]
    private Transform distortIconTrans = null;

    [SerializeField]
    private Vector2 triggerScale = new Vector2(1, 1);

    [SerializeField]
    private float triggerRot = default;

    [SerializeField]
    private float triggerTime = 1f;

    private bool triggeredAlert = false;

    private Coroutine alertCR = null;

    private PSO_CurrentGameMode currGameMode = null;

    //TODO: make an unscaled deltaTime SO that can share all this across where we do use it
    private bool gamePaused => currGameMode.Value == GameModeEnum.PAUSE;

    private float deltaTime => UnscaledTimeIfNotPaused(gamePaused); 
    
    private void Awake()
    {
        if (currTimer == null){return;}
        currEnergy.RegisterForPropertyChanged(OnValuesChange);
        currTimer.RegisterForPropertyChanged(OnValuesChange);
    }

    private void OnValuesChange(FillBarQuant _, FillBarQuant __)
    {
        bool available = 
            currTimer.Value.TransReached && currTimer.Value.Quant == 0 
            && currEnergy.Value.TransReached && currEnergy.Value.Quant >= armamentEnergyReq.Value;
        
        bool resetAlert = 
            currTimer.Value.Quant == 1 || currEnergy.Value.Quant < armamentEnergyReq.Value;

        if (available && !triggeredAlert)
        {
            triggeredAlert = true;
            SafeStartCoroutine(ref alertCR, TriggerAvailableEffect(), this);
        }
        
        if (resetAlert)
        {
            triggeredAlert = false;
        }
    }

    private IEnumerator TriggerAvailableEffect()
    {
        Vector2 startScale = distortIconTrans.localScale;
        float startRot = distortIconTrans.localRotation.eulerAngles.z;

        float perc = 0;
        while (perc < 1)
        {
            perc += deltaTime / triggerTime;
            float loopedPerc = perc < 0.5 ? perc * 2 : 2 - perc * 2;
            Vector2 scale = Vector2.Lerp(startScale, triggerScale, loopedPerc);
            float rot = Mathf.Lerp(startRot, triggerRot, loopedPerc);

            distortIconTrans.localScale = scale;
            distortIconTrans.localRotation = Quaternion.Euler(new Vector3(0, 0, rot));
            yield return null;
        }
    }
}
