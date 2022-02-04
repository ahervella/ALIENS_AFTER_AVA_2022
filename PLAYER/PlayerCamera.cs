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
    private SO_CameraAngle cachedTargetCamAngle = null;

    private SO_CameraAngle oldCameraAngle = null;
    private float tweenPerc;
    private Camera cam;

    private void Awake()
    {
        tweenPerc = 1;
        cam = GetComponent<Camera>();
        targetCameraAngle.RegisterForPropertyChanged(OnCameraAngleChange);
    }

    private void Start()
    {
        //just go to the current angle on load
        OnCameraAngleChange(null, targetCameraAngle.Value);
    }

    private void OnCameraAngleChange(SO_CameraAngle prevAngle, SO_CameraAngle newAngle)
    {
        StartCoroutine(CR_OnCameraAngleChange(prevAngle, newAngle));
    }

    private IEnumerator CR_OnCameraAngleChange(SO_CameraAngle prevAngle, SO_CameraAngle newAngle)
    {
        //if was in the middle of a tween
        if (tweenPerc != 1 && tweenPerc > 0)
        {
            //let it finish the last FixedUpdate loop,
            //and stop the current tween so we can wait for any delay before resuming
            yield return new WaitForFixedUpdate();
            tweenPerc = 1;
        }

        //also in case if was negative (should never be)
        if (targetCameraAngle.Value.TweenDelay > 0)
        {
            yield return new WaitForSeconds(targetCameraAngle.Value.TweenDelay);
        }


        SetCurrentTransformAsOldAngle();
        cachedTargetCamAngle = newAngle;
        //resume with updated old and new angle
        tweenPerc = 0;
    }


    private void SetCurrentTransformAsOldAngle()
    {
        float currFOV = cam.fieldOfView;
        Vector3 currPosOffset = transform.position - subjectTransform.position;

        oldCameraAngle = new SO_CameraAngle(currFOV, currPosOffset, transform.eulerAngles, 0, PlayerActionEnum.NONE);
    }

    private void Update()
    {
        TickCameraTween();
    }

    private void TickCameraTween()
    {
        if (tweenPerc >= 1)
        {
            return;
        }

        tweenPerc += Time.deltaTime;
        float easedTweenPerc = EasedPercent(tweenPerc);

        cam.fieldOfView = Mathf.Lerp(oldCameraAngle.FieldOfView, cachedTargetCamAngle.FieldOfView, easedTweenPerc);

        transform.position = subjectTransform.position + Vector3.Lerp(oldCameraAngle.PosOffset, cachedTargetCamAngle.PosOffset, easedTweenPerc);

        transform.rotation = Quaternion.Lerp(
            Quaternion.Euler(oldCameraAngle.Rot),
            Quaternion.Euler(cachedTargetCamAngle.Rot),
            easedTweenPerc);

        if (easedTweenPerc >= 1)
        {
            tweenPerc = 1;
        }
    }
}
