using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PSO_FillBarQuant", menuName = "ScriptableObjects/Property/PSO_FillBarQuant")]
public class PSO_FillBarQuant : PropertySO<FillBarQuant>
{
    public override void ModifyValue(FillBarQuant mod)
    {
        SetValue(mod);
    }

    public void ModifyValue(int quant, bool transReached, float transTime)
    {
        ModifyValue(new FillBarQuant(quant, transReached, transTime));
    }

    public void BarTransReached()
    {
        ModifyValue(new FillBarQuant(Value.Quant, true, Value.TransTime));
    }
}

[Serializable]
public class FillBarQuant
{
    [SerializeField]
    private int quant = default;
    public int Quant => quant;

    [SerializeField]
    private bool transReached = default;
    public bool TransReached => transReached;

    [SerializeField]
    private float transTime = 1;
    public float TransTime => transTime;

    public FillBarQuant(int quant, bool transReached, float transTime)
    {
        this.quant = quant;
        this.transReached = transReached;
        this.transTime = transTime;
    }
}
