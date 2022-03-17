using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private WeaponEnum weaponType;

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

    [SerializeField]
    private bool isAlienProjectile = false;

    private void Awake()
    {
        slope = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angleOffset), 0, Mathf.Cos(Mathf.Deg2Rad * angleOffset)) * speedPerSec;
        transform.position += posOffset;

        //TODO: is this too jank or can I just resort to using on collision or on trigger and return
        //depending on the isAlienProjectile flag?
        BoxCollider hitBox = GetComponent<BoxCollider>();
        hitBox.isTrigger = !isAlienProjectile;
    }

    private void Update()
    {
        transform.position += slope * Time.deltaTime;
    }

    public void OnEnteredHazard(TerrHazard hazard)
    {
        if (isAlienProjectile) { return; }


        Destroy(hazard.gameObject);

        MadeImpact();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerRunner player = other.gameObject.GetComponent<PlayerRunner>();
        if (player != null)
        {
            player.OnEnterProjectile(weaponType);
            MadeImpact();
            return;
        }
    }

    private void MadeImpact()
    {
        if (OnImpactPrefab != null)
        {
            GameObject instance = Instantiate(OnImpactPrefab, transform.parent);
            instance.transform.position = transform.position;
        }

        if (destroyOnImpact)
        {
            destructionSpritePrefab?.InstantiateDestruction(transform.parent, transform);
            Destroy(gameObject);
        }
    }
}
