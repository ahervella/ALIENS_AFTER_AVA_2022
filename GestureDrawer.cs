using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureDrawer : MonoBehaviour
{
    private LineRenderer line;

    private float zOffset = 10f;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();


    }

    public void renderShit(Vector2 firstPos, Vector2 secondPos)
    {
        Vector3[] positions = new Vector3[2];
        positions[0] = Camera.main.ScreenToWorldPoint(new Vector3(firstPos.x, firstPos.y, zOffset));
        //Debug.Log(firstPos);
        positions[1] = Camera.main.ScreenToWorldPoint(new Vector3(secondPos.x, secondPos.y, zOffset));
        line.positionCount = 2;
        line.SetPositions(positions);
    }
}
