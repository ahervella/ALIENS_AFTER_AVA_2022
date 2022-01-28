using System.Collections;
using UnityEngine;

public class AudioFades : MonoBehaviour
{
    public static IEnumerator FadeIn(AudioSource source, float fadeTime, float targetVol)
    {
        source.Play();
        source.volume = 0f;
        while (source.volume < targetVol)
        {
            source.volume += Time.deltaTime / fadeTime;
            yield return null;
        }
    }

    public static IEnumerator FadeOut(AudioSource source, float fadeTime)
    {
        float startVol = source.volume;
        while (source.volume > 0)
        {
            source.volume -= Time.deltaTime / fadeTime;
            yield return null;
        }
        source.Stop();
    }

    private static readonly int volIncrement = 30; //number of increments per second in each transition
    public static IEnumerator VolTransition(AudioSource source, float duration, float targetVol)
    {
        //Calculate value and length of each step
        int steps = (int)(volIncrement * duration);
        float stepTime = duration / steps;
        float stepSize = (targetVol - source.volume) / steps;

        //Actual fade code
        for (int i = 1; i < steps; i++)
        {
            source.volume += stepSize;
            yield return new WaitForSeconds(stepTime);
        }

        //assures final volume is accurate
        source.volume = targetVol;
    }

    private static readonly int lpIncrement = 30; //number of increments per second in each transition
    public static IEnumerator LPTransition(AudioLowPassFilter _lpFilter, float duration, float targetFreq)
    {
        //Calculate value and length of each step
        //Debug.Log("TRANSITIONING FILTER FREQ");
        int steps = (int)(lpIncrement * duration);
        float stepTime = duration / steps;
        float stepSize = (targetFreq - _lpFilter.cutoffFrequency) / steps;

        //Actual fade code
        for (int i = 1; i < steps; i++)
        {
            _lpFilter.cutoffFrequency += stepSize;
            yield return new WaitForSeconds(stepTime);
        }

        //assures final frequency is accurate
        _lpFilter.cutoffFrequency = targetFreq;
    }
}
