using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static public T Current = null;

    abstract protected T GetSelf();

    private void Awake()
    {
        if (Current == null)
        {
            Current = GetSelf();
        }
        else if (Current != GetSelf())
        {
            Destroy(gameObject);
        }
    }
}

