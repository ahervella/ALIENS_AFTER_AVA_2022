using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField]
    private int tileDistanceReach;

    //the visual grapple rope offset relative to its spawn location
    [SerializeField]
    private Vector2 visualOffset = default;

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

    [SerializeField]
    private SpriteRenderer grappleRope = null;

    [SerializeField]
    private SpriteRenderer grappleHook = null;

    [SerializeField]
    private GameObject grappleRopeContainer = null;

    private Coroutine grappleWindowCR = null;
    private Coroutine grappleRetractCR = null;
    private Coroutine grappleReelInCR = null;

    private void Awake()
    {
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

        if (grappleReelInCR != null)
        {
            StopCoroutine(grappleReelInCR);
            grappleReelInCR = null;
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

        //aim for center of floor height
        Vector3 raycastPos = new Vector3(transform.position.x, terrSettings.FloorHeight / 2, transform.position.z);

        bool raycastHit = false;
        RaycastHit raycastInfo = new RaycastHit();

        while (!raycastHit && currDist < maxDist)
        {
            yield return null;
            currDist += grappleSpeed * Time.deltaTime;

            Debug.DrawRay(raycastPos, Vector3.forward * currDist);
            raycastHit = Physics.Raycast(raycastPos, Vector3.forward, out raycastInfo, currDist, cachedMaskLayer);
            SetSpritePositions(currDist);
        }

        if (raycastHit)
        {
            //if layers set correctly, then we know this was an alien hit box
            ReelInTowardsAlien(raycastInfo.collider.gameObject);
            grappleWindowCR = null;
            yield break;
        }

        RetractGrapple(currDist);
        grappleWindowCR = null;
    }

    private void SetSpritePositions(float currDist)
    {
        if (currDist < 0) { return; }
        Vector3 targetPos = transform.position + Vector3.forward * currDist;
        Vector3 originPos = transform.position + new Vector3(visualOffset.x, visualOffset.y, 0);

        Vector3 normVect = targetPos - originPos;
        Vector3 normVectY = normVect + new Vector3(visualOffset.x, 0, 0);
        Vector3 normVectX = normVect + new Vector3(0, visualOffset.y, 0);

        grappleHook.transform.position = targetPos;

        grappleRope.size = new Vector2(normVect.magnitude, grappleRope.size.y);
        grappleRope.transform.localPosition = new Vector3(0, 0, normVect.magnitude / 2);

        grappleRopeContainer.transform.position = originPos;

        float xRot = Mathf.Rad2Deg * Mathf.Asin(visualOffset.y / normVectY.magnitude);
        float yRot = Mathf.Rad2Deg * Mathf.Asin(visualOffset.x / normVectX.magnitude);

        Debug.DrawRay(originPos - new Vector3(visualOffset.x, 0, 0), normVectY);
        Debug.DrawRay(originPos - new Vector3(0, visualOffset.y, 0), normVectX);

        grappleRopeContainer.transform.rotation = Quaternion.Euler(new Vector3(xRot, -yRot, 0));
    }

    private void ReelInTowardsAlien(GameObject target)
    {
        Debug.Log("reeling in towards alien");

        HazardAlien alienObj = null;
        while (alienObj == null)
        {
            alienObj = target.GetComponent<HazardAlien>();
            target = target.transform.parent.gameObject;
        }

        alienObj.Stun();

        //TODO: change animation? Or would player anim take care of that. Or at least
        //change grapple anim part
        speedChangeDelegate.InvokeDelegateMethod(treadmillSpeedChange);
        currAction.ModifyValue(PlayerActionEnum.GRAPPLE_REEL);

        grappleReelInCR = StartCoroutine(ReelInCoroutine(target));
    }

    private IEnumerator ReelInCoroutine(GameObject target)
    {
        float currDist;
        while (target.transform.position.z > transform.position.z)
        {
            yield return null;
            currDist = Vector3.Distance(target.transform.position, transform.position);
            SetSpritePositions(currDist);
        }
    }

    //TODO: do we want this or just a seperate prefab to play this out? (probably that...)
    private void RetractGrapple(float currDist)
    {
        Debug.Log("retracting grapple");
        //TODO: change grapple animation to retract
        grappleRetractCR = StartCoroutine(GrappleRetractCoroutine(currDist));
    }

    private IEnumerator GrappleRetractCoroutine(float currDist)
    {
        float retractSpeed = currDist / grappleRetractTime;
        while (currDist > 0)
        {
            yield return null;
            currDist -= retractSpeed * Time.deltaTime;
            SetSpritePositions(currDist);
        }

        Destroy(gameObject);
    }
}
