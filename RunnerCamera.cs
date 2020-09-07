using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerCamera : MonoBehaviour
{
    public Transform playerRef;

    public float smoothSpeed = 0.125f;
    public Quaternion rotationFromForward = new Quaternion(0.102783814f, 0, 0, 0.99470371f);
    public Vector3 offset;
    public float jumpOffset = -0.1f;
    public float jumpRotOffset = 10f;

    public float rollOffset;
    public float rollRotOffset;



    private void Start()
    {
        RunnerPlayer.onAnimationStarted += animStart;
        RunnerPlayer.onAnimationEnded += animEnd;
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, playerRef.position + offset, smoothSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationFromForward, smoothSpeed);
    }


    //TODO: make these switch statements? + apply spherical tween to lerp, maybe make the universal utilities class in RunnerGameObject?

    private void animStart(RunnerPlayer.PLAYER_STATE state)
    {
        if (state == RunnerGameObject.PLAYER_STATE.JUMP)
        {
            Vector3 rot2Euler = rotationFromForward.eulerAngles;
            rotationFromForward = Quaternion.Euler(rot2Euler.x - jumpRotOffset, rot2Euler.y, rot2Euler.z);

            offset += new Vector3(0, jumpOffset, 0);
        }


        if (state == RunnerGameObject.PLAYER_STATE.ROLL)
        {
            Vector3 rot2Euler = rotationFromForward.eulerAngles;
            rotationFromForward = Quaternion.Euler(rot2Euler.x - rollRotOffset, rot2Euler.y, rot2Euler.z);

            offset += new Vector3(0, rollOffset, 0);
        }
    }

    private void animEnd(RunnerPlayer.PLAYER_STATE state)
    {
        if (state == RunnerGameObject.PLAYER_STATE.JUMP)
        {
            Vector3 rot2Euler = rotationFromForward.eulerAngles;
            rotationFromForward = Quaternion.Euler(rot2Euler.x + jumpRotOffset, rot2Euler.y, rot2Euler.z);

            offset -= new Vector3(0, jumpOffset, 0);
        }

        if (state == RunnerGameObject.PLAYER_STATE.ROLL)
        {
            Vector3 rot2Euler = rotationFromForward.eulerAngles;
            rotationFromForward = Quaternion.Euler(rot2Euler.x + rollRotOffset, rot2Euler.y, rot2Euler.z);

            offset -= new Vector3(0, rollOffset, 0);
        }
    }

}
