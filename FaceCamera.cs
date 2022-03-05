using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField]
    private bool flipFacingSide = false;
    private void Update()
    {
        if (flipFacingSide)
        {
            transform.LookAt(Camera.main.transform);
            return;
        }
        //delta vector, in the opposite direction from the this objects transform
        transform.LookAt(-(Camera.main.transform.position - transform.position) + transform.position);
    }
}
