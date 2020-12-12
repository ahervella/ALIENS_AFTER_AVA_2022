using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectAwake : MonoBehaviour
{
    static public ScriptableObjectAwake Current = null;

    public static event System.Action OnInitialize = delegate { };

    private void Awake()
    {
        //if (Current == null)
        //{
        //    Current = this;
        //}
        //else if (Current != this)
        //{
        //    Destroy(gameObject);
        //}
        OnInitialize();
    }
}
