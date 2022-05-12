using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;

[RequireComponent(typeof(AudioWrapperSource))]
public class TussleManager : MonoBehaviour
{
    [SerializeField]
    private DSO_TreadmillSpeedChange treadmillSpeedDelegate = null;

    [SerializeField]
    private BoolDelegateSO energyBarDisplayDelegate = null;

    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private IntPropertySO currLives = null;

    [SerializeField]
    private VideoPlayer videoPlayer = null;
    private VideoPlayer videoPlayer2;

    private VideoPlayer currVideoPlayer;

    [SerializeField]
    private SO_TussleSettings settings = null;

    [SerializeField]
    private SO_DeveloperToolsSettings devTools = null;

    [SerializeField]
    private BoolDelegateSO tussleStartDelegate = null;

    [SerializeField]
    private BoolDelegateSO tussleResolveDebugDelegate = null;

    private AudioWrapperSource audioSource;

    private bool playerAdvantage;

    private TussleInputSequence currSequence;

    private Coroutine startCR = null;
    private Coroutine loopCR = null;

    private bool gamePaused => currGameMode.Value == GameModeEnum.PAUSE;

    private void Awake()
    {
        audioSource = GetComponent<AudioWrapperSource>();
        tussleStartDelegate.RegisterForDelegateInvoked(InitiateTussle);
        tussleResolveDebugDelegate.RegisterForDelegateInvoked(ResolveDebug);
        videoPlayer.targetCamera = Camera.main;
        videoPlayer.clip = null;

        //make the duplicate que videoplayer one at runtime to have the same settings
        videoPlayer2 = Instantiate(videoPlayer, transform);
    }

    private int InitiateTussle(bool playerAdvantage)
    {
        currGameMode.RegisterForPropertyChanged(OnGameModeChange);

        videoPlayer.enabled = true;
        videoPlayer2.enabled = true;

        this.playerAdvantage = playerAdvantage;

        treadmillSpeedDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(0, 0));
        energyBarDisplayDelegate.InvokeDelegateMethod(false);

        Time.timeScale = 0;

        TussleVideoWrapper startVidWrapper = settings.GetTussleVideoWrapper(playerAdvantage ? TussleVideoType.ADV_START : TussleVideoType.DIS_START);
        TussleVideoWrapper loopVidWrapper = settings.GetTussleVideoWrapper(playerAdvantage ? TussleVideoType.ADV_LOOP : TussleVideoType.DIS_LOOP);

        void startVideoLoadedCallback() => settings.TussleHazardCleanUpDelegate.InvokeDelegateMethod(true);

        startCR = StartCoroutine(PlayVideo(startVidWrapper, startVideoLoadedCallback));
        loopCR = StartCoroutine(WaitForCurrVideoAndPlay(loopVidWrapper));

        InitiateButtonSequence();
        return 0;
    }

    private void OnGameModeChange(GameModeEnum oldMode, GameModeEnum newMode)
    {
        if (newMode == GameModeEnum.PAUSE)
        {
            currVideoPlayer.Pause();
        }
        if (oldMode == GameModeEnum.PAUSE && newMode == GameModeEnum.PLAY)
        {
            currVideoPlayer.Play();
        }
    }

    //TODO: base class these video methods to use with the main menu later

    private IEnumerator PlayVideo(TussleVideoWrapper vidWrapper, Action callbackOnFinish = null)
    {
        VideoPlayer player = QueueTussleVideo(vidWrapper);
        currVideoPlayer = player;
        player.Play();

        yield return WaitForVideoToLoad(player);

        vidWrapper.AudioWrapper?.PlayAudioWrapper(audioSource);

        if (vidWrapper.TakeDamageAfterDelay)
        {
            StartCoroutine(TakeDamageCR(vidWrapper.DamageDelay));
        }

        if (callbackOnFinish != null)
        {
            yield return WaitForCurrVideoToFinish();
            callbackOnFinish();
        }
    }

    private IEnumerator TakeDamageCR(float delay)
    {
        yield return TussleWaitForSeconds(delay);
        if (!devTools.Invincibility)
        {
            currLives.ModifyValue(-1);
        }
    }

    private IEnumerator TussleWaitForSeconds(float time)
    {
        float currTime = 0;
        while (currTime < time)
        {
            if (!gamePaused)
            {
                currTime += Time.unscaledDeltaTime;
            }
            yield return null;
        }
    }

    private IEnumerator WaitForCurrVideoToFinish()
    {
        while (currVideoPlayer.isPlaying || gamePaused)
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

        queuedWrapper.AudioWrapper?.PlayAudioWrapper(audioSource);

        currVideoPlayer.clip = null;
        currVideoPlayer = queuedPlayer;
    }

    private void InitiateButtonSequence()
    {
        StartCoroutine(InitiateButtonSequenceCR());
    }

    private IEnumerator InitiateButtonSequenceCR()
    {
        yield return TussleWaitForSeconds(settings.ShowSequenceDelay);

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

    private int ResolveDebug(bool successful)
    {
        StopCoroutine(startCR);
        StopCoroutine(loopCR);
        currSequence?.ResolveSequence(successful);
        return 0;
    }

    public void EndTussle()
    {
        currGameMode.DeRegisterForPropertyChanged(OnGameModeChange);
        Time.timeScale = 1;
        treadmillSpeedDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(1, 0));
        energyBarDisplayDelegate.InvokeDelegateMethod(true);

        currAction.ModifyValue(PlayerActionEnum.RUN);

        Destroy(currSequence.gameObject);
        currVideoPlayer = null;
        videoPlayer.clip = null;
        videoPlayer2.clip = null;
    }
}
