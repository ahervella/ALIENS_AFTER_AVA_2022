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

    PropertyChanged OnPropertyChanged;

    PropertyChanged OnGameModeSceneUnloadedCleanUp;

    public T Value => currentValue;

    private void OnEnable()
    {
        currentValue = startingValue;

        if (triggerChangeWithStartVal)
        {
            OnPropertyChanged?.Invoke(currentValue, currentValue);
        }
    }

    public T RegisterForPropertyChanged(PropertyChanged method, bool persistant = false)
    {
        //deregister first because this revoes all method instance
        //of this name, such that we make sure each method is registered
        //only once
        OnPropertyChanged -= method;
        OnPropertyChanged += method;

        if (!persistant)
        {
            RegisterForGameModeSceneUnloaded(method);
        }

        return Value;
    }

    private void RegisterForGameModeSceneUnloaded(PropertyChanged method)
    {
        if (!registeredWithGameModeManager)
        { 
            S_GameModeManager.Current.RegisterForGameModeSceneUnloaded(S_GameModeManager_OnGameModeSceneUnloaded);
            registeredWithGameModeManager = true;
        }

        OnGameModeSceneUnloadedCleanUp -= method;
        OnGameModeSceneUnloadedCleanUp += method;
    }

    private void S_GameModeManager_OnGameModeSceneUnloaded()
    {
        OnPropertyChanged -= OnGameModeSceneUnloadedCleanUp;
        OnGameModeSceneUnloadedCleanUp = null;
    }

    public void DeRegisterForPropertyChanged(PropertyChanged method)
    {
        OnPropertyChanged -= method;
        OnGameModeSceneUnloadedCleanUp -= method;
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