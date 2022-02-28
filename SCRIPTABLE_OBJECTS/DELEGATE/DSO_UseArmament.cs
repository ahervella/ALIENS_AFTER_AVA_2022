using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DSO_UseArmament", menuName = "ScriptableObjects/Delegates/DSO_UseArmament")]
public class DSO_UseArmament : ScriptableObject
{
    //TODO: convert something like this into an abstract DSO class to use
    //as we do with PSOs, and convert the lane change to it

    public delegate bool OnUseArmamentRequest(AArmament armament);

    OnUseArmamentRequest OnArmamentUse;

    public void RegisterResultMethod(OnUseArmamentRequest resultMethod)
    {
        OnArmamentUse = resultMethod;
    }

    public bool TryUseArmament(AArmament armament)
    {
        if (OnArmamentUse == null)
        {
            Debug.LogError("No Armament use result method set in PSO_UseArmament");
        }

        return OnArmamentUse(armament);
    }
}
