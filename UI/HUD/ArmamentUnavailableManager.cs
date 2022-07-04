using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmamentUnavailableManager : MonoBehaviour
{
    [SerializeField]
    private IntPropertySO armamentEnergyReq = null;

    [SerializeField]
    private PSO_CurrentEnergy currEnergy = null;

    [SerializeField]
    private PSO_FillBarQuant currTimer = null;

    [SerializeField]
    private GameObject availableMask = null;

    private void Start()
    {
        currEnergy.RegisterForPropertyChanged(OnValuesChange);
        currTimer.RegisterForPropertyChanged(OnValuesChange);
    }

    private void OnValuesChange(FillBarQuant _, FillBarQuant __)
    {
        availableMask.SetActive(
            !(currTimer.Value.TransReached && currTimer.Value.Quant == 0 
            && currEnergy.Value.TransReached && currEnergy.Value.Quant >= armamentEnergyReq.Value));
    }
}
