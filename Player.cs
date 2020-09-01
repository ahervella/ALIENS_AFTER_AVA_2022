using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : TerrObject
{
    int lives;


    // Start is called before the first frame update
    void Start()
    {
        objType = OBJ_TYPE.PLAYER;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider coll)
    {
        TerrObject terrObj = coll.gameObject.GetComponent<TerrObject>();
        if (terrObj == null) { return;  }

        if (terrObj.objType != OBJ_TYPE.ENEMY && terrObj.objType == OBJ_TYPE.STATIC_HAZ) { return; }

        Debug.Log("IM HIT!!!!");
    }
}
