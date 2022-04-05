using System;
using UnityEngine;

[Serializable]
public abstract class PropertySO<T> : PropertySO
{

    [SerializeField]
    private T startingValue = default;

    [SerializeField]
    private bool triggerChangeWithStartVal = true;

    [NonSerialized]
    private T currentValue;

    //This one shot is used so we don't have to rely
    //on the ScriptableObject OnEnable which has had mixed results in the past
    [NonSerialized]
    private bool registeredWithGameModeManager = false;

    public delegate void PropertyChanged(T oldValue, T newValue);

    private PropertyChanged OnPropertyChanged;

    private PropertyChanged OnGameModeSceneUnloadedPersistance;

    public T Value => currentValue;

    private void OnEnable()
    {
        currentValue = startingValue;

        if (triggerChangeWithStartVal)
        {
            OnPropertyChanged?.Invoke(currentValue, currentValue);
        }
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