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
    private IntPropertySO currTimer = null;

    [SerializeField]
    private GameObject availableMask = null;

    private void Awake()
    {
        currEnergy.RegisterForPropertyChanged(OnValuesChange);
        currTimer.RegisterForPropertyChanged(OnValuesChange);
    }

    private void OnValuesChange(int _, int __)
    {
        availableMask.SetActive(
            !(currTimer.Value == 0
            && currEnergy.Value >= armamentEnergyReq.Value));
    }
}
