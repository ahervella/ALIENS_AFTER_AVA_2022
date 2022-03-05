using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private GameObject OnImpactPrefab = null;

    [SerializeField]
    private float angleOffset = 0f;
    [SerializeField]
    private Vector3 posOffset = default;
    private Vector3 slope;

    [SerializeField]
    private float speedPerSec = 10;

    [SerializeField]
    private bool destroyOnImpact = true;

    [SerializeField]
    private DestructionSprite destructionSpritePrefab = null;

    private void Awake()
    {
        slope = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angleOffset), 0, Mathf.Cos(Mathf.Deg2Rad * angleOffset)) * speedPerSec;
        transform.position += posOffset;
    }

    private void Update()
    {
        transform.position += slope * Time.deltaTime;
    }

    public void OnEnteredHazard(TerrHazard hazard)
    {
        if (OnImpactPrefab != null)
        {
            GameObject instance = Instantiate(OnImpactPrefab, transform.parent);
            instance.transform.position = transform.position;
        }

        Destroy(hazard.gameObject);

        if (destroyOnImpact)
        {
            destructionSpritePrefab?.InstantiateDestruction(transform.parent, transform);
            Destroy(gameObject);
        }
    }
}
