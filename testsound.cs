using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testsound : MonoBehaviour
{
    public List<AudioClip> audioClips1 = new List<AudioClip>();
    public List<AudioClip> audioClips2 = new List<AudioClip>();
    private AudioSource mySource;
    private void Awake()
    {
        mySource = GetComponent<AudioSource>();
        if (mySource == null)
        {
            mySource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Start()
    {
        StartCoroutine(nameof(WaitASecondAndShoot));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitASecondAndShoot()
    {
        if (!mySource.isPlaying) //used for the first time, before a clip has been assigned, just use the random time value.
        {
            Debug.Log("wait a sec");
            yield return new WaitForSeconds(1);
        }

        else // Once a clip has been assigned, add the clip’s length to the random time interval for the wait between clips.
        {
            Debug.Log("wait 2 secs");
            yield return new WaitForSeconds(2);
        }

        playSounds();
    }

    void playSounds()
    {
        AudioClip chosenClip1 = audioClips1[Random.Range(0, audioClips1.Count - 1)]; //These three lines prevent immediate repeats of audio samples
        audioClips1.Remove(chosenClip1);
        audioClips1.Add(chosenClip1);
        mySource.PlayOneShot(chosenClip1);
        
        AudioClip chosenClip2 = audioClips2[Random.Range(0, audioClips2.Count - 1)]; //These three lines prevent immediate repeats of audio samples
        audioClips2.Remove(chosenClip2);
        audioClips2.Add(chosenClip2);
        mySource.PlayOneShot(chosenClip2);
        StartCoroutine(nameof(WaitASecondAndShoot));
        Debug.Log(chosenClip1.name);
        Debug.Log(chosenClip2.name);
    }

}
