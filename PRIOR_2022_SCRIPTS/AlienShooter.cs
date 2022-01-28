using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienShooter : MonoBehaviour
{
    [SerializeField]
    private float delayPerShot;

    [SerializeField]
    private RunnerThrowable bulletObject;

    [SerializeField]
    private float randomDelayRange;

    [SerializeField]
    private Transform spawnLocation;

    [SerializeField]
    private AAudioWrapper gunSound;

    //used by animation event
    private void StartGunFire()
    {
        StartCoroutine(DelayedGunFire());
    }

    private IEnumerator DelayedGunFire()
    {
        FireGun();
        float randomVal = Random.Range(0, randomDelayRange);
        yield return new WaitForSecondsRealtime(randomVal);
        StartCoroutine(FireNextBullet());
    }

    private IEnumerator FireNextBullet()
    {
        yield return new WaitForSecondsRealtime(delayPerShot);

        FireGun();
        
        StartCoroutine(FireNextBullet());
    }

    private void FireGun()
    {
        if (spawnLocation == null)
        {
            bulletObject.Instantiate(transform.position, transform);
            RunnerSounds.Current.PlayAudioWrapper(gunSound, gameObject);
        }
        else
        {
            bulletObject.Instantiate(spawnLocation.position, spawnLocation);
            RunnerSounds.Current.PlayAudioWrapper(gunSound, gameObject);
        }
    }
}
