using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerTools;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System;

public class TussleManager : MonoBehaviour
{
    [SerializeField]
    private SpriteAnim tussleAnim = null;

    [SerializeField]
    private DSO_TreadmillSpeedChange treadmillSpeedDelegate = null;

    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;

    [SerializeField]
    private VideoPlayer videoPlayer = null;
    private VideoPlayer videoPlayer2;

    [SerializeField]
    private SO_TussleSettings settings = null;

    [SerializeField]
    private BoolDelegateSO tussleStartDelegate = null;

    private void Awake()
    {
        tussleStartDelegate.SetInvokeMethod(InitiateTussle);
        videoPlayer.targetCamera = Camera.main;

        //to have the same settings
        videoPlayer2 = Instantiate(videoPlayer, transform);
    }

    private int InitiateTussle(bool playerAdvantage)
    {
        treadmillSpeedDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(0, 0));

        TussleVideoWrapper startVidWrapper = settings.GetTussleVideoWrapper(playerAdvantage ? TussleVideoType.ADV_START : TussleVideoType.DIS_START);
        TussleVideoWrapper loopVidWrapper = settings.GetTussleVideoWrapper(playerAdvantage ? TussleVideoType.ADV_LOOP : TussleVideoType.DIS_LOOP);
        VideoPlayer startVideoPlayer = PlayTussleVideo(startVidWrapper);
        VideoPlayer loopVideoPlayer = QueueTussleVideo(loopVidWrapper);

        StartCoroutine(TussleVideoTransitionCoroutine(startVideoPlayer, loopVideoPlayer));// () => PlayTussleVideo(loopVidWrapper)));

        InitiateButtonSequence();
        return 0;
    }

    private VideoPlayer PlayTussleVideo(TussleVideoWrapper wrapper)
    {
        if (videoPlayer.isPlaying)
        {
            SwapVideoPlayers();
        }

        videoPlayer.clip = wrapper.Video;
        videoPlayer.isLooping = wrapper.Loop;
        videoPlayer.Play();
        return videoPlayer;
    }

    private void SwapVideoPlayers()
    {
        VideoPlayer temp = videoPlayer2;
        videoPlayer2 = videoPlayer;
        videoPlayer = temp;
    }

    private VideoPlayer QueueTussleVideo(TussleVideoWrapper wrapper)
    {
        if (videoPlayer2.isPlaying)
        {
            SwapVideoPlayers();
        }

        videoPlayer2.clip = wrapper.Video;
        videoPlayer2.isLooping = wrapper.Loop;
        return videoPlayer2;
    }

    private IEnumerator TussleVideoTransitionCoroutine(VideoPlayer currVideo, VideoPlayer queuedVideo)
    {
        yield return null;
        while (!currVideo.isPrepared || currVideo.isPlaying)
        {
            yield return null;
        }

        queuedVideo.Play();
    }

    private IEnumerator InitiateButtonSequence()
    {
        yield break;
    }

    private IEnumerator tempTussleCoroutine()
    {
        yield return new WaitForSeconds(2);
        EndTussle();
    }

    private void EndTussle()
    {
        treadmillSpeedDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(1, 0));
        currAction.ModifyValue(PlayerActionEnum.RUN);
        Destroy(gameObject);
    }
}
