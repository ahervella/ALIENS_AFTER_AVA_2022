using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

        if (!IsAudioSourceNonLoopAndActive(cachedAudioWrapperSource))
        {
            Destroy(gameObject);
            return;
        }

        List<Component> filteredComponents = FilterNecessaryComponents(gameObject.GetComponents<Component>());
        SafelyDestroyComponents(filteredComponents);


        for (int i = 0; i < transform.childCount; i++)
        {
            HelperUtil.SafeDestroy(transform.GetChild(i).gameObject);
        }

        StartCoroutine(WaitForAudioToFinish());
    }


    private List<Component> FilterNecessaryComponents(Component[] components)
    {
        List<Component> componentsList = new List<Component>(components);
        foreach (Component c in components)
        {
            if (c is Transform
                    || c is SafeAudioWrapperSource
                    || c is AudioWrapperSource)
            {
                componentsList.Remove(c);
                continue;
            }

            //looped audio should be stopped immediately
            if (c is AudioSource aSource)
            {
                if (!aSource.loop)
                {
                    componentsList.Remove(c);
                }
            }
        }
        return componentsList;
    }

    /// <summary>
    /// Recursively iterates and removes components in the correct order
    /// dictated by their required components so as no to recieve any errors
    /// </summary>
    /// <param name="components"></param>
    private void SafelyDestroyComponents(List<Component> components)
    {
        //https://answers.unity.com/questions/1445663/how-to-auto-remove-the-component-that-was-required.html

        if (components.Count == 0)
        {
            return;
        }

        if (components.Count == 1)
        {
            Destroy(components[0]);
            return;
        }

        //Find the head of the tree

        List<Component> potentialHeads = new List<Component>(components);
        List<Component> remainingNodes = new List<Component>();

        foreach (Component c in components)
        {
            RemoveReqFromHeadList(c, potentialHeads, remainingNodes);
        }

        //gauranteed heads at this point
        foreach (Component c in potentialHeads)
        {
            Destroy(c);
        }

        SafelyDestroyComponents(remainingNodes);
    }

    private void RemoveReqFromHeadList(Component component, List<Component> potentialHeads, List<Component> remainingNodes)
    {
        MemberInfo memberInfo = component.GetType();
        RequireComponent[] requiredComponentsAtts = Attribute.GetCustomAttributes(memberInfo, typeof(RequireComponent), true) as RequireComponent[];

        foreach (RequireComponent rc in requiredComponentsAtts)
        {
            if (rc != null)
            {
                Component reqComp = component.GetComponent(rc.m_Type0);
                if (reqComp != null && potentialHeads.Contains(reqComp))
                {
                    potentialHeads.Remove(reqComp);
                    remainingNodes.Add(reqComp);
                    continue;
                }
            }
        }
    }

    private IEnumerator WaitForAudioToFinish()
    {
        yield return null;

        while (IsAudioSourceNonLoopAndActive(cachedAudioWrapperSource))
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
