using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using static AudioUtil;

[RequireComponent(typeof(SafeAudioWrapperSource))]
public class GrappleHook : MonoBehaviour
{
    [SerializeField]
    private Equipment grappleEquipmentSO = null;

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
    private SO_LayerSettings layerSettings = null;

    [SerializeField]
    private SpriteRenderer grappleRope = null;

    [SerializeField]
    private GameObject grappleHook = null;

    [SerializeField]
    private GameObject grappleRopeContainer = null;

    [SerializeField]
    private AAudioWrapperV2 grapplingSnagAudio = null;

    [SerializeField]
    private BoolDelegateSO tussleInitDelegate = null;

    private bool cachedTussleStarted = false;

    private AudioWrapperSource audioSource;

    private Coroutine grappleWindowCR = null;
    private Coroutine grappleRetractCR = null;
    private Coroutine grappleReelInCR = null;

    private float currDist = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioWrapperSource>();

        grappleOnFlag.ModifyValue(true);
        grappleOnFlag.RegisterForPropertyChanged(OnGrappleFlagChange);
        tussleInitDelegate.RegisterForDelegateInvoked(OnTussleInitDelegate);
        currAction.RegisterForPropertyChanged(OnActionChange);
        grappleWindowCR = StartCoroutine(GrappleWindowCoroutine());
    }

    //TODO change this delegate to a PSO to not have to do this
    private int OnTussleInitDelegate(bool _)
    {
        cachedTussleStarted = true;
        return 0;
    }

    private void OnActionChange(PlayerActionEnum oldAction, PlayerActionEnum newAction)
    {
        //whether because we got hit, reached the alien and going into a tussle
        //or changed action prior to reeling in, stop the whole thing
        if (newAction != PlayerActionEnum.GRAPPLE_REEL
            && !grappleEquipmentSO.ApplicableActions.Contains(newAction))
        {
            RetractGrapple();
        }
    }

    private void OnGrappleFlagChange(bool oldVal, bool newVal)
    {
        //In case we need something else to stop the grapple
        if (!newVal)
        {
            SafeDestroy(gameObject);
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
        currDist = 0;
        float grappleSpeed = maxDist / grappleTimeWindow;

        //aim for center of floor height
        Vector3 raycastPos = new Vector3(transform.position.x, terrSettings.FloorHeight / 2, transform.position.z);

        //TODO: do we even really need to keep this optimization? For reward boxes?
        int maskLayer = 1 << layerSettings.HitBoxLayer;

        bool nonjumpableHazardHit = false;

        List<GameObject> checkedObjects = new List<GameObject>();

        while (!nonjumpableHazardHit && currDist < maxDist)
        {
            yield return null;
            currDist += grappleSpeed * Time.deltaTime;
            SetSpritePositions(currDist);

            Debug.DrawRay(raycastPos, Vector3.forward * currDist);
            RaycastHit[] hits = Physics.RaycastAll(raycastPos, Vector3.forward, currDist, maskLayer);

            foreach(RaycastHit hit in hits)
            {
                if (checkedObjects.Contains(hit.collider.gameObject)) { continue; }

                if (hit.collider.GetComponent<BoxColliderSP>() is BoxColliderSP hitBox)
                {
                    if (hitBox.RootParent.GetComponent<HazardAlien>() is HazardAlien alien)
                    {
                        ReelInTowardsAlien(alien);
                        grappleWindowCR = null;
                        yield break;
                    }

                    if (hitBox.RootParent.GetComponent<TerrHazard>() is TerrHazard hazard)
                    {
                        if (hazard.GetRequiredAvoidAction(hitBox) != PlayerActionEnum.JUMP)
                        {
                            nonjumpableHazardHit = true;
                            break;
                        }

                        checkedObjects.Add(hazard.gameObject);
                    }
                }
            }
        }

        RetractGrapple();
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

    private void ReelInTowardsAlien(HazardAlien alien)
    {
        StopAllAudioSourceSounds(audioSource);

        grapplingSnagAudio.PlayAudioWrapper(audioSource);

        Debug.Log("reeling in towards alien");

        alien.Stun();

        //TODO: change animation? Or would player anim take care of that. Or at least
        //change grapple anim part
        speedChangeDelegate.InvokeDelegateMethod(treadmillSpeedChange);
        currAction.ModifyValue(PlayerActionEnum.GRAPPLE_REEL);

        grappleReelInCR = StartCoroutine(ReelInCoroutine(alien.gameObject));
    }

    private IEnumerator ReelInCoroutine(GameObject target)
    {
        float currDist;
        //second part of condition here in case tussle video that triggers
        //the start is taking a while
        while (!cachedTussleStarted && target.transform.position.z > transform.position.z)
        {
            currDist = Vector3.Distance(target.transform.position, transform.position);
            SetSpritePositions(currDist);
            yield return null;
        }
    }

    //TODO: do we want this or just a seperate prefab to play this out? (probably that...)
    private void RetractGrapple()
    {
        SafeStopCoroutine(ref grappleWindowCR, this);
        Debug.Log("retracting grapple");
        //TODO: change grapple animation to retract
        SafeStartCoroutine(ref grappleRetractCR, GrappleRetractCoroutine(), this);
    }

    private IEnumerator GrappleRetractCoroutine()
    {
        float maxDist = tileDistanceReach * terrSettings.TileDims.y;
        float retractSpeed = maxDist / grappleRetractTime;
        while (currDist > 0)
        {
            currDist -= retractSpeed * Time.deltaTime;
            SetSpritePositions(currDist);
            yield return null;
        }

        SafeDestroy(gameObject);
    }
}
