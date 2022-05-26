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

    private PlayerActionEnum requiredAvoidAction = PlayerActionEnum.NULL;

    //hack to make sure we only get the req action if its enabled
    public PlayerActionEnum RequiredAvoidAction =>
        !BoxDisabled ? requiredAvoidAction : PlayerActionEnum.NULL;

    public bool BoxDisabled
    {
        set
        {
            enabled = value;
        }

        get
        {
            return !isActiveAndEnabled;
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
        if (BoxDisabled) { return; }
        onTriggerEnterMethod?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (BoxDisabled) { return; }
        onTriggerExitMethod?.Invoke(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (BoxDisabled) { return; }
        onColliderEnterMethod?.Invoke(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (BoxDisabled) { return; }
        onColliderExitMethod?.Invoke(collision);
    }
}
