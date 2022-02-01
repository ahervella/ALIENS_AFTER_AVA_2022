using System;
using UnityEngine;

[Serializable]
public abstract class PropertySO<T> : PropertySO
{
    public delegate void PropertyChanged(T oldValue, T newValue);

    PropertyChanged OnPropertyChanged;

    [SerializeField]
    private T startingValue = default(T);

    [NonSerialized]
    private T currentValue;

    public T Value => currentValue;

    private void OnEnable()
    {
        ResetToStart();
    }

    public T RegisterForPropertyChanged(PropertyChanged method)
    {
        //deregister first because this revoes all method instance
        //of this name, such that we make sure each method is registered
        //only once
        OnPropertyChanged -= method;
        OnPropertyChanged += method;
        return Value;
    }

    public void DeRegisterForPropertyChanged(PropertyChanged method)
    {
        OnPropertyChanged -= method;
    }

    protected void SetValue(T newValue)
    {
        T oldValue = currentValue;
        currentValue = newValue;
        OnPropertyChanged?.Invoke(oldValue, newValue);
    }

    public abstract void ModifyValue(T mod);

    public void ResetToStart()
    {
        SetValue(startingValue);
    }
}


public abstract class PropertySO : ScriptableObject
{
    [SerializeField]
    public string Name = null;
    [SerializeField]
    protected string Description = null;

    [SerializeField]
    public Sprite icon = null;
}