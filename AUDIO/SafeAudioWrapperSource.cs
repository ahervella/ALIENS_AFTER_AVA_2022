using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioUtil;

[RequireComponent(typeof(AudioWrapperSource))]
public class SafeAudioWrapperSource : MonoBehaviour
{
    [SerializeField]
    private AAudioWrapperV2 onAwakeAudio = null;

    [SerializeField]
    private AAudioWrapperV2 onDestroyAudio = null;

    private AudioWrapperSource cachedAudioWrapperSource;

    private void Awake()
    {
        cachedAudioWrapperSource = GetComponent<AudioWrapperSource>();
        onAwakeAudio?.PlayAudioWrapper(cachedAudioWrapperSource);
    }
    public void SafeDestroy()
    {
        //if it is not null, it will be part of the active audio sources list below
        onDestroyAudio?.PlayAudioWrapper(cachedAudioWrapperSource);

        if (!IsAudioSourceActive(cachedAudioWrapperSource))
        {
            Destroy(gameObject);
            return;
        }


        foreach(Component c in gameObject.GetComponents<Component>())
        {
            if (c is Transform || c == this || c is AudioWrapperSource) { continue; }

            Destroy(c);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            HelperUtil.SafeDestroy(transform.GetChild(i).gameObject);
        }

        StartCoroutine(WaitForAudioToFinish());
    }

    private IEnumerator WaitForAudioToFinish()
    {
        yield return null;

        while (IsAudioSourceActive(cachedAudioWrapperSource))
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
