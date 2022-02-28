using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DelegateSO<PARAM_TYPE, RETURN_TYPE> : ScriptableObject
{
    public delegate RETURN_TYPE InvokeDelegate(PARAM_TYPE param);

    InvokeDelegate OnInvokeDelegateMethod;

    public void SetInvokeMethod(InvokeDelegate invokeMethod)
    {
        OnInvokeDelegateMethod = invokeMethod;
    }

    public virtual RETURN_TYPE InvokeDelegateMethod(PARAM_TYPE param)
    {
        if (OnInvokeDelegateMethod == null)
        {
            Debug.Log($"Invoke method for DelegateSO {name} not set!");
            return default;
        }

        return OnInvokeDelegateMethod(param);
    }
}
