using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SafeAudioWrapperSource))]
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
    private AAudioWrapperV2 travelAudio = null;

    [SerializeField]
    private AAudioWrapperV2 impactAudio = null;

    [SerializeField]
    private bool isAlienProjectile = false;

    private AudioWrapperSource audioSource;

    private void Awake()
    {
        slope = (isAlienProjectile ? -1 : 1) * speedPerSec * new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * angleOffset),
            0,
            Mathf.Cos(Mathf.Deg2Rad * angleOffset));

        transform.position += posOffset;

        //TODO: is this too jank or can I just resort to using on collision or on trigger and return
        //depending on the isAlienProjectile flag?
        BoxCollider hitBox = GetComponent<BoxCollider>();
        hitBox.isTrigger = !isAlienProjectile;

        audioSource = GetComponent<AudioWrapperSource>();
        travelAudio?.PlayAudioWrapper(audioSource);
    }

    private void Update()
    {
        transform.position += slope * Time.deltaTime;
    }

    public void OnEnteredHazard(TerrHazard hazard)
    {
        if (isAlienProjectile) { return; }


        SafeDestroy(hazard.gameObject);

        MadeImpact();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerRunner player = other.gameObject.GetComponent<PlayerRunner>();
        if (player != null)
        {
            //We give the player the audio so its from their source,
            //and so we don't prematurely delete this source object
            player.OnEnterProjectile(weaponType, /*impactAudio, */out bool dodged);

            if (dodged) { MadeImpact(); }
            return;
        }
    }

    private void MadeImpact()
    {
        impactAudio?.PlayAudioWrapper(audioSource);

        if (OnImpactPrefab != null)
        {
            GameObject instance = Instantiate(OnImpactPrefab, transform.parent);
            instance.transform.position = transform.position;
        }

        if (destroyOnImpact)
        {
            destructionSpritePrefab?.InstantiateDestruction(transform.parent, transform);
            SafeDestroy(gameObject);
        }
    }
}
