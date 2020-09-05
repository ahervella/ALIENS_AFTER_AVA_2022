using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GestureDrawer : MonoBehaviour
{
    private LineRenderer line;

    private float zOffset = 10f;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        RunnerControls.OnInputAction += renderShit;
    }

    public void renderShit(RunnerControls.InputData inputData)
    {
        if (inputData.getStartPos() == inputData.getEndPos()) { return; }

        Vector3[] positions = new Vector3[2];
        positions[0] = Camera.main.ScreenToWorldPoint(new Vector3(inputData.getStartPos().x, inputData.getStartPos().y, zOffset));
        positions[1] = Camera.main.ScreenToWorldPoint(new Vector3(inputData.getEndPos().x, inputData.getEndPos().y, zOffset));
        line.positionCount = 2;
        line.SetPositions(positions);
    }
}
