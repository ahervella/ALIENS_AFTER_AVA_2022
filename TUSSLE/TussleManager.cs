using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;

public class TussleManager : MonoBehaviour
{
    [SerializeField]
    private DSO_TreadmillSpeedChange treadmillSpeedDelegate = null;

    [SerializeField]
    private BoolDelegateSO energyBarDisplayDelegate = null;

    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;

    [SerializeField]
    private VideoPlayer videoPlayer = null;
    private VideoPlayer videoPlayer2;

    private VideoPlayer currVideoPlayer;

    [SerializeField]
    private SO_TussleSettings settings = null;

    [SerializeField]
    private BoolDelegateSO tussleStartDelegate = null;

    private bool playerAdvantage;

    private TussleInputSequence currSequence;

    private void Awake()
    {
        tussleStartDelegate.SetInvokeMethod(InitiateTussle);
        videoPlayer.targetCamera = Camera.main;
        videoPlayer.clip = null;

        //make the duplicate que videoplayer one at runtime to have the same settings
        videoPlayer2 = Instantiate(videoPlayer, transform);
    }

    private int InitiateTussle(bool playerAdvantage)
    {
        videoPlayer.enabled = true;
        videoPlayer2.enabled = true;

        this.playerAdvantage = playerAdvantage;

        treadmillSpeedDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(0, 0));
        energyBarDisplayDelegate.InvokeDelegateMethod(false);

        TussleVideoWrapper startVidWrapper = settings.GetTussleVideoWrapper(playerAdvantage ? TussleVideoType.ADV_START : TussleVideoType.DIS_START);
        TussleVideoWrapper loopVidWrapper = settings.GetTussleVideoWrapper(playerAdvantage ? TussleVideoType.ADV_LOOP : TussleVideoType.DIS_LOOP);
        StartCoroutine(PlayVideo(startVidWrapper));
        StartCoroutine(WaitForCurrVideoAndPlay(loopVidWrapper));

        InitiateButtonSequence();
        return 0;
    }

    //TODO: base class these video methods to use with the main menu later

    private IEnumerator PlayVideo(TussleVideoWrapper vidWrapper, Action callbackOnFinish = null)
    {
        VideoPlayer player = QueueTussleVideo(vidWrapper);
        currVideoPlayer = player;
        player.Play();

        yield return WaitForVideoToLoad(player);

        if (callbackOnFinish != null)
        {
            yield return WaitForCurrVideoToFinish();
            callbackOnFinish();
        }
    }

    private IEnumerator WaitForCurrVideoToFinish()
    {
        while (currVideoPlayer.isPlaying)
        {
            yield return null;
        }
    }

    private VideoPlayer QueueTussleVideo(TussleVideoWrapper wrapper)
    {
        if (currVideoPlayer == videoPlayer)
        {
            //Swap video players
            VideoPlayer temp = videoPlayer2;
            videoPlayer2 = videoPlayer;
            videoPlayer = temp;
        }

        videoPlayer.clip = wrapper.Video;
        videoPlayer.isLooping = wrapper.Loop;
        return videoPlayer;
    }

    private IEnumerator WaitForVideoToLoad(VideoPlayer player)
    {
        player.Prepare();
        while (!player.isPrepared)
        {
            yield return null;
        }
    }

    private IEnumerator WaitForCurrVideoAndPlay(TussleVideoWrapper queuedWrapper)
    {
        VideoPlayer queuedPlayer = QueueTussleVideo(queuedWrapper);

        yield return WaitForVideoToLoad(queuedPlayer);
        yield return WaitForVideoToLoad(currVideoPlayer);
        yield return WaitForCurrVideoToFinish();

        queuedPlayer.Play();

        yield return WaitForVideoToLoad(queuedPlayer);

        currVideoPlayer.clip = null;
        currVideoPlayer = queuedPlayer;
    }

    private void InitiateButtonSequence()
    {
        currSequence = Instantiate(settings.GetInputSequencePrefab(playerAdvantage), transform);
        currSequence.StartSequence(SequenceResolved);
    }

    private void SequenceResolved(bool successful)
    {
        TussleVideoWrapper wrapper = successful ?
            settings.GetTussleVideoWrapper(playerAdvantage ? TussleVideoType.ADV_WIN : TussleVideoType.DIS_WIN)
            : settings.GetTussleVideoWrapper(playerAdvantage ? TussleVideoType.ADV_LOSE : TussleVideoType.DIS_LOSE);

        StartCoroutine(PlayVideo(wrapper, EndTussle));
    }

    public void EndTussle()
    {
        treadmillSpeedDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(1, 0));
        energyBarDisplayDelegate.InvokeDelegateMethod(true);

        currAction.ModifyValue(PlayerActionEnum.RUN);

        Destroy(currSequence.gameObject);
        currVideoPlayer = null;
        videoPlayer.clip = null;
        videoPlayer2.clip = null;
    }
}
