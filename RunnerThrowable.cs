﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class RunnerThrowable : MonoBehaviour
{
    public enum THROW_TYPE { BULLET, ROCK, GUN};
    const float GRAV = 4.89f;

    TerrObject terrObj;

    public THROW_TYPE throwType;

    public Transform playerTrans;

    public Vector3 spawnOffset;

    public Vector2 throwVelocity;
    Vector3 vel;

    public bool applyGrav;

    public float dist2Disappear;

    public static event System.Action<RunnerThrowable> throwableGenerated = delegate { };
    public static event System.Action<RunnerThrowable> throwableDestroyed = delegate { };

    public RunnerThrowable Instantiate(Transform playerTrans)
    {
        this.playerTrans = playerTrans;
        RunnerThrowable inst = Instantiate(this);
        throwableGenerated(inst);
        return inst;

    }

    void Start()
    {
        terrObj = GetComponent<TerrObject>();

        transform.position = playerTrans.position + spawnOffset;
        vel = new Vector3(0, throwVelocity.y, throwVelocity.x);
    }

    private void Update()
    {
        if (transform.position.z - playerTrans.position.z >= dist2Disappear)
        {
            //Debug.Log("throwable destroyed");
            //throwableDestroyed(this);
            Destroy(gameObject);
            return;
        }

        transform.position += vel * Time.deltaTime;

        if (applyGrav) { vel.y -= GRAV * Time.deltaTime; }// / RunnerGameObject.getGameFPS(); }
    }

    private void OnDestroy()
    {
        Debug.Log("throwable destroyed");
        throwableDestroyed(this);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        TerrObject terrObj = other.gameObject.GetComponent<TerrObject>();
        if (terrObj == null) { return; }

        //destroy any hazard if its a bullet, else only aliens if its anything else (rock or thrown gun)
        if (terrObj.objType == TerrObject.OBJ_TYPE.ENEMY || (terrObj.objType == TerrObject.OBJ_TYPE.STATIC_HAZ && throwType == THROW_TYPE.BULLET))
        {
            Debug.Log(("went through {0}", terrObj.gameObject.name));
            Destroy(terrObj.gameObject);
        }
    }
}