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

    //used by animation event
    private void StartGunFire()
    {
        StartCoroutine(DelayedGunFire());
    }

    private IEnumerator DelayedGunFire()
    {
        float randomVal = Random.Range(0, randomDelayRange);
        yield return new WaitForSecondsRealtime(randomVal);
        StartCoroutine(FireNextBullet());
    }

    private IEnumerator FireNextBullet()
    {
        yield return new WaitForSecondsRealtime(delayPerShot);
        if (spawnLocation == null)
        {
            bulletObject.Instantiate(transform.position, transform);
        }
        else
        {
            bulletObject.Instantiate(spawnLocation.position, spawnLocation);
        }
        
        StartCoroutine(FireNextBullet());
    }
}
