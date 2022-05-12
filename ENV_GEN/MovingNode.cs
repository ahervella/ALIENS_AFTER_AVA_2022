using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class MovingNode : MonoBehaviour
{
    [SerializeField]
    protected SO_TerrSettings terrSettings = null;

    [SerializeField]
    private float speedPerSec = 10;

    [SerializeField]
    private float angleOffset = 0f;

    protected Vector3 slope;


    private void Awake()
    {
        slope = speedPerSec * new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * angleOffset),
            0,
            Mathf.Cos(Mathf.Deg2Rad * angleOffset));

        OnAwake();
    }

    protected virtual void OnAwake() { }

    private void Update()
    {
        transform.position += slope * Time.deltaTime;
        CheckIfOutOfVerticalBounds();
    }

    private void CheckIfOutOfVerticalBounds()
    {
        //Destroy if one row behind 0 (which is when rows reset for terrNode)
        //or if further than last row
        if ((slope.z < 0 && transform.position.z < -terrSettings.TileDims.y)
            || (slope.z > 0 && transform.position.z > terrSettings.TileRows * terrSettings.TileDims.y))
        {
            SafeDestroy(gameObject);
        }
    }
}
