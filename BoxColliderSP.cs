using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderSP : MonoBehaviour
{
    Action<Collider> onTriggerMethod = null;
    Action<Collision> onColliderMethod = null;

    private BoxCollider box;
    public BoxCollider Box()
    {
        if (box == null)
        {
            box = GetComponent<BoxCollider>();
        }
        return box;
    }

    public void SetOnTriggerMethod(Action<Collider> method)
    {
        onTriggerMethod = method;
    }

    public void SetOnCollisionMethod(Action<Collision> method)
    {
        onColliderMethod = method;
    }

    private void OnTriggerEnter(Collider other)
    {
        onTriggerMethod?.Invoke(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        onColliderMethod?.Invoke(collision);
    }
}
