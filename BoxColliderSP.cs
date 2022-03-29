using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderSP : MonoBehaviour
{
    Action<Collider> onTriggerEnterMethod = null;
    Action<Collider> onTriggerExitMethod = null;

    Action<Collision> onColliderEnterMethod = null;
    Action<Collision> onColliderExitMethod = null;

    private BoxCollider box;
    public BoxCollider Box()
    {
        if (box == null)
        {
            box = GetComponent<BoxCollider>();
        }
        return box;
    }

    public void SetOnTriggerEnterMethod(Action<Collider> method)
    {
        onTriggerEnterMethod = method;
    }

    public void SetOnTriggerExitMethod(Action<Collider> method)
    {
        onTriggerExitMethod = method;
    }

    public void SetOnCollisionEnterMethod(Action<Collision> method)
    {
        onColliderEnterMethod = method;
    }

    public void SetOnCollisionExitMethod(Action<Collision> method)
    {
        onColliderExitMethod = method;
    }


    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnterMethod?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExitMethod?.Invoke(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        onColliderEnterMethod?.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        onColliderExitMethod?.Invoke(collision);
    }
}
