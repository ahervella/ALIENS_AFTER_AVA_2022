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
    private TreadmillSpeedChange treadmillSpeedChange = null;

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

    [SerializeField]
    private int grappleLayer = 8;
    private int cachedMaskLayer;

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


        //aim for center of floor height
        float y = terrSettings.FloorHeight / 2;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        cachedMaskLayer = 1 << grappleLayer;

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
        Debug.Log("destroying grapple");
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
        Debug.Log("started grapple");
        float maxDist = tileDistanceReach * terrSettings.TileDims.y;
        float currDist = 0;
        float grappleSpeed = maxDist / grappleTimeWindow;

        bool raycastHit = false;
        RaycastHit raycastInfo = new RaycastHit();

        while (!raycastHit && currDist < maxDist)
        {
            yield return null;
            currDist += grappleSpeed * Time.deltaTime;

            Debug.DrawRay(transform.position, Vector3.forward * currDist);
            raycastHit = Physics.Raycast(transform.position, Vector3.forward, out raycastInfo, currDist, cachedMaskLayer);
        }

        if (raycastHit)
        {
            //if layers set correctly, then we know this was an alien hit box
            ReelInTowardsAlien(raycastInfo.collider.gameObject);
            grappleWindowCR = null;
            yield break;
        }

        RetractGrapple();
        grappleWindowCR = null;
    }

    private void ReelInTowardsAlien(GameObject target)
    {
        Debug.Log("reeling in towards alien");
        //TODO: change animation? Or would player anim take care of that. Or at least
        //change grapple anim part
        speedChangeDelegate.InvokeDelegateMethod(treadmillSpeedChange);
        currAction.ModifyValue(PlayerActionEnum.GRAPPLE_REEL);
    }

    //TODO: do we want this or just a seperate prefab to play this out? (probably that...)
    private void RetractGrapple()
    {
        Debug.Log("retracting grapple");
        //TODO: change grapple animation to retract
        grappleRetractCR = StartCoroutine(GrappleRetractCoroutine());
    }

    private IEnumerator GrappleRetractCoroutine()
    {
        yield return new WaitForSeconds(grappleRetractTime);
        Destroy(gameObject);
    }



}
