using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderSP : MonoBehaviour
{
    [SerializeField]
    private MonoBehaviour rootParent = null;
    public MonoBehaviour RootParent => rootParent;

    [SerializeField]
    private bool isNodeHitBox = false;

    [SerializeField]
    private SO_LayerSettings layerSettings = null;

    [SerializeField]
    private bool cacheObjsOnDisable = false;

    [SerializeField]
    private bool useLocalPos = false;
    public bool UseLocalPos => useLocalPos;

    private List<Collider> cachedColliders = new List<Collider>();
    private List<Collision> cachedCollisions = new List<Collision>();

    private PlayerActionEnum requiredAvoidAction = PlayerActionEnum.NULL;

    //hack to make sure we only get the req action if its enabled
    public PlayerActionEnum RequiredAvoidAction =>
        !BoxDisabled ? requiredAvoidAction : PlayerActionEnum.NULL;

    public bool BoxDisabled
    {
        set
        {
            Box().enabled = !value;
            OnBoxDisabledChange(enabled);
        }

        get
        {
            return !Box().enabled;
        }
    }

    public void OnBoxDisabledChange(bool boxEnabled)
    {
        if (!cacheObjsOnDisable) { return; }
        if (enabled)
        {
            if (onTriggerEnterMethod != null)
            {
                foreach (Collider c in cachedColliders)
                {
                    onTriggerEnterMethod.Invoke(c);
                }
            }

            if (onColliderEnterMethod != null)
            {
                foreach (Collision c in cachedCollisions)
                {
                    onColliderEnterMethod.Invoke(c);
                }
            }
        }

        else
        {
            if (onTriggerExitMethod != null)
            {
                foreach (Collider c in cachedColliders)
                {
                    onTriggerExitMethod.Invoke(c);
                }
            }

            if (onColliderExitMethod != null)
            {
                foreach (Collision c in cachedCollisions)
                {
                    onColliderExitMethod.Invoke(c);
                }
            }
        }
    }

    public void SetReqAvoidAction(PlayerActionEnum action)
    {
        requiredAvoidAction = action;
    }

    //private PlayerActionEnum requiredTakeDownAction = PlayerActionEnum.NULL;

    //public PlayerActionEnum RequiredTakeDownAction =>
    //    isActiveAndEnabled ? requiredTakeDownAction : PlayerActionEnum.NULL;

    //public void SetTakeDownAction(PlayerActionEnum action)
    //{
    //    requiredTakeDownAction = action;
    //}

    Action<Collider> onTriggerEnterMethod = null;
    Action<Collider> onTriggerExitMethod = null;

    Action<Collision> onColliderEnterMethod = null;
    Action<Collision> onColliderExitMethod = null;

    private void Awake()
    {
        SetAsNodeHitBox(isNodeHitBox);
    }

    public void SetAsNodeHitBox(bool set)
    {
        if (set)
        {
            gameObject.layer = layerSettings.HitBoxLayer;
        }
        else
        {
            gameObject.layer = 0;
        }
    }

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
        if (BoxDisabled || onTriggerEnterMethod == null) { return; }

        onTriggerEnterMethod.Invoke(other);

        if (cacheObjsOnDisable)
        {
            cachedColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (BoxDisabled || onTriggerExitMethod == null) { return; }

        onTriggerExitMethod.Invoke(other);
        if (cacheObjsOnDisable)
        {
            cachedColliders.Remove(other);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (BoxDisabled || onColliderEnterMethod == null) { return; }

        onColliderEnterMethod.Invoke(collision);
        if (cacheObjsOnDisable)
        {
            cachedCollisions.Add(collision);
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (BoxDisabled || onColliderExitMethod == null) { return; }

        onColliderExitMethod.Invoke(collision);
        if (cacheObjsOnDisable)
        {
            cachedCollisions.Remove(collision);
        }
    }
}
