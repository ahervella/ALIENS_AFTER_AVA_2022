using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class TerrObject : MonoBehaviour
{
    public enum OBJ_TYPE { STATIC, STATIC_HAZ, ENEMY, PLAYER};

    public OBJ_TYPE objType = OBJ_TYPE.STATIC;

    protected Animator anim;
    public BoxCollider hitBox;
    public bool canFlip = true;

    public float appearanceLikelihood = 0.2f;


    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        hitBox = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
