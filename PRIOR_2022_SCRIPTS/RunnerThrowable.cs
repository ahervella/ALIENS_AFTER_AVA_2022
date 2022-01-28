using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class RunnerThrowable : MonoBehaviour
{
    public enum THROW_TYPE { BULLET, ENEMY_BULLET, ROCK, GUN};
    const float GRAV = 4.89f;

    private TerrObject terrObj;

    public THROW_TYPE throwType;

    //public Transform playerTrans;
    private Vector3 startPos;

    public Vector3 spawnOffset;

    public Vector2 throwVelocity;
    private Vector3 vel;

    public bool applyGrav;

    public float dist2Disappear;

    public static event System.Action<RunnerThrowable> ThrowableGenerated = delegate { };
    public static event System.Action<RunnerThrowable> ThrowableDestroyed = delegate { };

    //TODO: make this inherit a TerrObj?
    public RunnerThrowable Instantiate(Vector3 startPos, Transform parent = null)
    {
        
        RunnerThrowable inst = parent == null? Instantiate(this) : Instantiate(this, parent);
        inst.startPos = startPos;
        inst.transform.position = startPos + spawnOffset;
        inst.terrObj = GetComponent<TerrObject>();
        inst.vel = new Vector3(0, throwVelocity.y, throwVelocity.x);

        ThrowableGenerated(inst);
        return inst;

    }
    /*
    void Start()
    {
        terrObj = GetComponent<TerrObject>();
        vel = new Vector3(0, throwVelocity.y, throwVelocity.x);
    }
    */
    private void Update()
    {
        //dunno which direction its traveling in
        if (Mathf.Abs(transform.position.z - startPos.z) >= dist2Disappear)
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
        //Debug.Log("throwable destroyed");
        ThrowableDestroyed(this);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        TerrObject terrObj = other.gameObject.GetComponent<TerrObject>();
        if (terrObj == null) { return; }

        //destroy any hazard if its a bullet, else only aliens if its anything else (rock or thrown gun)
        if ((terrObj.objType == TerrObject.OBJ_TYPE.ENEMY && throwType != THROW_TYPE.ENEMY_BULLET)
            || (terrObj.objType == TerrObject.OBJ_TYPE.STATIC_HAZ && throwType == THROW_TYPE.BULLET))
        {
            //Debug.Log(("went through {0}", terrObj.gameObject.name));
            Destroy(terrObj.gameObject);
        }
    }
}
