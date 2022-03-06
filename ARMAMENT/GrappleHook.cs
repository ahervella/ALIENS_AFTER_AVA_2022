using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField]
    private int tileDistanceReach;

    [SerializeField]
    private float grappleTimeWindow;

    [SerializeField]
    private float grappleRetractTime;

    [SerializeField]
    private TreadmillSpeedChange grappleSpeedChange = null;

    [SerializeField]
    private DSO_TreadmillSpeedChange speedChangeDelegate = null;

    //Used in case we want to avoid other equipment use or weapons
    //while shooting grapple (before it grabs anything)
    [SerializeField]
    private BoolPropertySO grappleOnFlag = null;

    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;

    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    private Coroutine grappleWindowCR = null;
    private Coroutine grappleRetractCR = null;

    private void Awake()
    {
        //if state change into anything (say got hit)
        //cancel coroutines and the whole thing

        //cast a ray
        //time to hit something, else bail
        //if touches something other than alien, bail
        //if hit, change anim or player, change speed, change camera angle

        grappleOnFlag.ModifyValue(true);
        grappleOnFlag.RegisterForPropertyChanged(OnGrappleFlagChange);
        currAction.RegisterForPropertyChanged(OnActionChange);
        grappleWindowCR = StartCoroutine(GrappleWindowCoroutine());
    }

    private void OnActionChange(PlayerActionEnum oldAction, PlayerActionEnum newAction)
    {
        //whether because we got hit, reached the alien and going into a tussle
        //or changed action prior to reeling in, stop the whole thing
        if (newAction != PlayerActionEnum.GRAPPLE_REEL)
        {
            Destroy(gameObject);
        }
    }

    private void OnGrappleFlagChange(bool oldVal, bool newVal)
    {
        if (!newVal)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (grappleWindowCR != null)
        {
            StopCoroutine(grappleWindowCR);
            grappleWindowCR = null;
        }

        if (grappleRetractCR != null)
        {
            StopCoroutine(grappleRetractCR);
            grappleRetractCR = null;
        }

        grappleOnFlag.ModifyValue(false);
        grappleOnFlag.DeRegisterForPropertyChanged(OnGrappleFlagChange);
        currAction.DeRegisterForPropertyChanged(OnActionChange);
    }

    private IEnumerator GrappleWindowCoroutine()
    {
        float dist = tileDistanceReach * terrSettings.TileDims.y;
        bool raycastHit = false;
        float passedTime = 0;
        RaycastHit raycastInfo = new RaycastHit();

        while (!raycastHit && passedTime < grappleTimeWindow)
        {
            passedTime += Time.deltaTime;
            raycastHit = Physics.Raycast(transform.position, Vector3.forward, out raycastInfo, dist);
        }

        if (raycastHit)
        {
            HazardAlien alien = raycastInfo.collider.gameObject.GetComponent<HazardAlien>();
            if (alien != null)
            {
                ReelInTowardsAlien(alien);
                grappleWindowCR = null;
                return null;
            }
        }

        RetractGrapple();
        grappleWindowCR = null;
        return null;
    }

    private void ReelInTowardsAlien(HazardAlien alien)
    {
        //change animation? Or would player anim take care of that. Or at least
        //change grapple anim part
        speedChangeDelegate.InvokeDelegateMethod(grappleSpeedChange);
        currAction.ModifyValue(PlayerActionEnum.GRAPPLE_REEL);
    }

    private void RetractGrapple()
    {
        //change grapple animation to retract
        grappleRetractCR = StartCoroutine(GrappleRetractCoroutine());
    }

    private IEnumerator GrappleRetractCoroutine()
    {
        yield return new WaitForSeconds(grappleRetractTime);
        Destroy(gameObject);
    }



}
