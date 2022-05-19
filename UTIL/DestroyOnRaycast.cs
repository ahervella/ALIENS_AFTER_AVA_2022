using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class DestroyOnRaycast : MonoBehaviour
{
    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    [SerializeField]
    private SO_LayerSettings layerSettings = null;

    [SerializeField]
    private GameObject rootNode = null;

    [SerializeField]
    private BoxColliderSP ownHitBox = null;

    [SerializeField]
    private float tileDistCheck = 1f;

    [SerializeField]
    private List<PlayerActionEnum> destroyOnReqAction = new List<PlayerActionEnum>();

    private Vector3 raycastPosOffset;

    private void Update()
    {
        CheckRaycast();
    }

    private void Start()
    {
        raycastPosOffset = ownHitBox.Box().center;
        raycastPosOffset -= new Vector3(0, 0, ownHitBox.Box().size.z / 2f + 0.01f);
    }

    private void CheckRaycast()
    {
        //Vector3 raycastPos = new Vector3(transform.position.x, terrSettings.FloorHeight / 2, transform.position.z);

        float dist = tileDistCheck * terrSettings.TileDims.y;

        int maskLayer = 1 << layerSettings.HitBoxLayer;

        Vector3 raycastPos = ownHitBox.transform.position + raycastPosOffset;

        Debug.DrawRay(raycastPos, -Vector3.forward * dist);
        RaycastHit[] hits = Physics.RaycastAll(raycastPos, -Vector3.forward, dist, maskLayer);

        foreach (RaycastHit hit in hits)
        {
            BoxColliderSP hitBox = hit.collider.gameObject.GetComponent<BoxColliderSP>();

            if (hitBox != null && hitBox.RootParent is TerrHazard hazard)
            {
                if (destroyOnReqAction.Contains(hazard.GetRequiredAvoidAction(hitBox)))
                {
                    SafeDestroy(rootNode);
                    return;
                }
            }
        }
    }

}
