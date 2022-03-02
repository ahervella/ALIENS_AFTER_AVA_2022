using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbOscillation : MonoBehaviour
{
    //TODO: configure this and the settings script to have more dynamic
    //movements for when the player gets hit or when we fire a projectile;

    [SerializeField]
    private SO_OrbOscillationSettings settings;

    private Coroutine oscillationCR = null;
    private float oscillationTweenPerc = 1f;
    private Vector3 cachedOGLocalPosition = default;

    private void Awake()
    {
        cachedOGLocalPosition = transform.localPosition;
        StartOscillation();
    }

    private void StartOscillation()
    {
        if (oscillationCR != null)
        {
            StopCoroutine(oscillationCR);
        }

        oscillationCR = StartCoroutine(OscillationCoroutine());
    }

    private IEnumerator OscillationCoroutine()
    {
        while (true)
        {
            oscillationTweenPerc += Time.deltaTime / settings.OscillationTime;
            oscillationTweenPerc %= 1f;
            float yDelta = Mathf.Sin(oscillationTweenPerc * 2 * Mathf.PI) * settings.OscillationDisplacement;
            transform.localPosition = cachedOGLocalPosition + new Vector3(0, yDelta, 0);
            yield return null;
        }
    }
}
