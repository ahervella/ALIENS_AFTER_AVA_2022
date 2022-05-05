using System;
using UnityEngine;

[Serializable]
public abstract class PropertySO<T> : PropertySO
{

    [SerializeField]
    private T startingValue = default;

    [SerializeField]
    private bool triggerChangeWithStartVal = true;

    //[SerializeField]
    //private bool setStartingValOnFirstGet = false;

    [NonSerialized]
    private bool startValSet = false;

    [NonSerialized]
    protected T currentValue;

    [ReadOnly, SerializeField]
    private T inpsectorCurrValue;

    [NonSerialized]
    private bool invokeFlag = true;

    //This one shot is used so we don't have to rely
    //on the ScriptableObject OnEnable which has had mixed results in the past
    [NonSerialized]
    private bool registeredWithGameModeManager = false;

    //[NonSerialized]
    //private bool startingValSet = false;

    public delegate void PropertyChanged(T oldValue, T newValue);

    private PropertyChanged OnPropertyChanged;

    private PropertyChanged OnGameModeSceneUnloadedPersistance;

    public T Value => ValueGetter();

    //TODO: test if this is wokring in build before
    //reverting to setStartingValOnFirstGet
    private void OnEnable()
    {
        //currentValue = startingValue;
        //inpsectorCurrValue = startingValue;
        if (triggerChangeWithStartVal)
        {
            startValSet = true;
            currentValue = startingValue;
            OnPropertyChanged?.Invoke(currentValue, currentValue);
        }
    }

    public override string ValueToString()
    {
        return Value?.ToString();
    }

    protected virtual T ValueGetter()
    {
        if (!startValSet)
        {
            currentValue = startingValue;
        }
        return currentValue;
    }

    public T RegisterForPropertyChanged(PropertyChanged method, bool persistent = false)
    {
        if (!registeredWithGameModeManager && !persistent)
        {
            S_GameModeManager.Current.RegisterForGameModeSceneUnloaded(S_GameModeManager_OnGameModeSceneUnloaded);
            registeredWithGameModeManager = true;
        }

        //deregister first because this revoes all method instance
        //of this name, such that we make sure each method is registered
        //only once
        OnPropertyChanged -= method;
        OnPropertyChanged += method;

        if (persistent)
        {
            RegisterForGameModeSceneUnloaded(method);
        }

        return Value;
    }

    private void RegisterForGameModeSceneUnloaded(PropertyChanged method)
    {
        OnGameModeSceneUnloadedPersistance -= method;
        OnGameModeSceneUnloadedPersistance += method;
    }

    private void S_GameModeManager_OnGameModeSceneUnloaded()
    {
        //TODO: look more into why we were getting IndexOutOfRange
        //exceptions here when performing the reverese (saving the
        //we want to unregister, as done successfully in the InputManager)
        //And for DelegatePSOs as well;
        OnPropertyChanged = null;
        if (OnGameModeSceneUnloadedPersistance != null)
        {
            OnPropertyChanged = OnGameModeSceneUnloadedPersistance;
            return;
        }

        OnGameModeSceneUnloadedPersistance = null;
    }

    public void DeRegisterForPropertyChanged(PropertyChanged method)
    {
        OnPropertyChanged -= method;
        OnGameModeSceneUnloadedPersistance -= method;
    }

    //TODO: set up a lasInvokedValue in case we want the last value that was invoked as
    //the oldValue?
    protected void SetValue(T newValue)
    {
        //in case we set the value using the default 
        startValSet = true;

        if (invokeFlag)
        {
            T oldValue = currentValue;
            currentValue = newValue;
            inpsectorCurrValue = newValue;
            OnPropertyChanged?.Invoke(oldValue, newValue);
        }
        else
        {
            currentValue = newValue;
            inpsectorCurrValue = newValue;
        }

        TriggerDevMenuUpdate();
    }

    public abstract void ModifyValue(T mod);

    public void ModifyValueNoInvoke(T mod)
    {
        invokeFlag = false;
        ModifyValue(mod);
        invokeFlag = true;
    }

    public void ResetToStart()
    {
        SetValue(startingValue);
    }
}


public abstract class PropertySO : ScriptableObject
{
    private event System.Action DevMenu_PSOUpdated = delegate { };

    public abstract string ValueToString();

    public void DevMenu_SubscribeToPSO(Action devToolMethod)
    {
        DevMenu_PSOUpdated -= devToolMethod;
        DevMenu_PSOUpdated += devToolMethod;
    }

    protected void TriggerDevMenuUpdate()
    {
        DevMenu_PSOUpdated?.Invoke();
    }
}