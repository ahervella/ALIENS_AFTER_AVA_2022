using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class Player : MonoBehaviour
{
    public int lives;

    protected Animator anim;
    protected AnimatorOverrideController animOC;

    public BoxCollider hitBox;


    // Start is called before the first frame update
    void OnEnable()
    {
        hitBox = gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider coll)
    {
        TerrObject terrObj = coll.gameObject.GetComponent<TerrObject>();
        if (terrObj == null) { return;  }

        if (terrObj.objType != TerrObject.OBJ_TYPE.ENEMY && terrObj.objType == TerrObject.OBJ_TYPE.STATIC_HAZ) { return; }

        Debug.Log("IM HIT!!!!");
    }
}
