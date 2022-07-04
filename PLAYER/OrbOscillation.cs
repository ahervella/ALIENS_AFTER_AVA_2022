using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class OrbOscillation : MonoBehaviour
{
    //TODO: configure this and the settings script to have more dynamic
    //movements for when the player gets hit or when we fire a projectile;

    [SerializeField]
    private SO_OrbOscillationSettings settings = null;

    //[SerializeField]
    //private GameObject oscillationChild = null;

    [SerializeField]
    private float highShotDeltaYPos = 0f;

    [SerializeField]
    private PSO_CurrentPlayerAction currPlayerAction = null;

    private Vector3 cachedOGLocalPosition;

    private Coroutine upDownCR = null;
    private float currYPosDelta = 0f;
    private Vector3 upDownPosDelta = new Vector3(0, 0, 0);
    private bool targetUpOrDown = false;

    private Coroutine oscillationCR = null;
    private float oscillationTweenPerc = 1f;
    private Vector3 oscillationPosDelta = new Vector3(0, 0, 0);
    //private Vector3 cachedOGOscillationLocalPosition = default;

    private void Start()
    {
        cachedOGLocalPosition = transform.localPosition;
        //cachedOGOscillationLocalPosition = oscillationChild.transform.localPosition;

        currPlayerAction.RegisterForPropertyChanged(OnPlayerActionChange);

        StartOscillation();
    }

    private void StartOscillation()
    {
        if (oscillationCR != null)
        {
            StopCoroutine(oscillationCR);
        }

        oscillationCR = StartCoroutine(OscillationCoroutine());
    }

    private IEnumerator OscillationCoroutine()
    {
        while (true)
        {
            oscillationTweenPerc += Time.deltaTime / settings.OscillationTime;
            oscillationTweenPerc %= 1f;
            float yDelta = Mathf.Sin(oscillationTweenPerc * 2 * Mathf.PI) * settings.OscillationDisplacement;
            oscillationPosDelta = new Vector3(0, yDelta, 0);
            //oscillationChild.transform.localPosition = cachedOGOscillationLocalPosition + new Vector3(0, yDelta, 0);
            yield return null;
        }
    }

    private void OnPlayerActionChange(PlayerActionEnum _, PlayerActionEnum newAction)
    {
        if (newAction == PlayerActionEnum.JUMP)
        {
            if (targetUpOrDown)
            {
                return;
            }
            SafeStartCoroutine(ref upDownCR, MoveOrbUpDown(true), this);
        }
        else
        {
            if (!targetUpOrDown)
            {
                return;
            }
            SafeStartCoroutine(ref upDownCR, MoveOrbUpDown(false), this);
        }
    }

    private IEnumerator MoveOrbUpDown(bool moveUpOrDown)
    {
        targetUpOrDown = moveUpOrDown;
        float perc = 0;
        float targetStart = currYPosDelta;
        float targetEnd = moveUpOrDown ? highShotDeltaYPos : 0;
        //float currDelta = transform.position.y - cachedOGLocalPosition.y;
        while (perc < 1)
        {

            perc += Time.deltaTime / settings.UpDownMoveTime;


            currYPosDelta = Mathf.Lerp(targetStart, targetEnd, EasedPercent(perc));

            upDownPosDelta = new Vector3(0, currYPosDelta, 0);

            yield return null;
        }
    }

    private void Update()
    {
        transform.localPosition = cachedOGLocalPosition + upDownPosDelta + oscillationPosDelta;
    }
}
