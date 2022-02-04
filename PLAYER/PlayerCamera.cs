using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static HelperUtil;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Transform subjectTransform = null;

    [SerializeField]
    private PSO_TargetCameraAngle targetCameraAngle = null;

    private CameraAngle oldCameraAngle = null;
    private float tweenPerc;

    private Coroutine delayCR;

    private Camera cam;

    private void Awake()
    {
        tweenPerc = 1;
        cam = GetComponent<Camera>();
        targetCameraAngle.RegisterForPropertyChanged(OnCameraAngleChange);

        SetCameraStartPosition();
    }

    private void SetCameraStartPosition()
    {

        delayCR = null;
        transform.position = subjectTransform.position;
        transform.LookAt(subjectTransform);

        SetCurrentStateAsOldCamera();
    }

    private void SetCurrentStateAsOldCamera()
    {
        if (delayCR != null)
        {
            //didn't even get to start the tween, so just keep the old angle
            //and stop the delay CR
            StopCoroutine(delayCR);
            return;
        }

        //if we were half way, make a new camera angle that is the current state
        float currFOV = cam.fieldOfView;
        Vector3 currPosOffset = transform.position;
        Vector3 currRotOffset = transform.eulerAngles;
        oldCameraAngle = new CameraAngle(currFOV, currPosOffset, currRotOffset, 0);
    }

    private void OnCameraAngleChange(CameraAngle prevAngle, CameraAngle newAngle)
    {
        //if was in the middle of a tween
        if (tweenPerc != 1 && tweenPerc > 0)
        {
            SetCurrentStateAsOldCamera();
        }

        //if not we just use the current oldCameraAngle

        delayCR = StartCoroutine(StartAngleChange());
    }

    private IEnumerator StartAngleChange()
    {
        //also in case if was negative (should never be)
        if (targetCameraAngle.Value.TweenDelay > 0)
        {
            yield return new WaitForSeconds(targetCameraAngle.Value.TweenDelay);
        }

        delayCR = null;
        tweenPerc  = 0;
    }

    private void FixedUpdate()
    {
        TickCameraTween();
    }

    private void TickCameraTween()
    {
        if (tweenPerc >= 1)
        {
            return;
        }

        tweenPerc += Time.fixedDeltaTime;
        float easedTweenPerc = EasedPercent(tweenPerc);
        cam.fieldOfView = Mathf.Lerp(oldCameraAngle.FieldOfView, targetCameraAngle.Value.FieldOfView, easedTweenPerc);

        float x = Mathf.Lerp(oldCameraAngle.PosOffset.x, targetCameraAngle.Value.PosOffset.x, easedTweenPerc);
        float y = Mathf.Lerp(oldCameraAngle.PosOffset.y, targetCameraAngle.Value.PosOffset.y, easedTweenPerc);
        float z = Mathf.Lerp(oldCameraAngle.PosOffset.z, targetCameraAngle.Value.PosOffset.z, easedTweenPerc);
        transform.position = subjectTransform.position;
        transform.position += new Vector3(x, y, z);

        //TODO: are the angles in radians or degrees, affect LerpAngle vs Lerp?
        x = Mathf.Lerp(oldCameraAngle.RofOffset.x, targetCameraAngle.Value.RofOffset.x, easedTweenPerc);
        y = Mathf.Lerp(oldCameraAngle.RofOffset.y, targetCameraAngle.Value.RofOffset.y, easedTweenPerc);
        z = Mathf.Lerp(oldCameraAngle.RofOffset.z, targetCameraAngle.Value.RofOffset.z, easedTweenPerc);

        transform.LookAt(subjectTransform);
        transform.eulerAngles += new Vector3(x, y, z);

        if (easedTweenPerc >= 1)
        {
            tweenPerc = 1;
        }
    }
}
