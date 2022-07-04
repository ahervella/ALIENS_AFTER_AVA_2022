using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;
using UnityEngine.UI;
using static HelperUtil;

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
    private BoolDelegateSO bossTussleDamageDSO = null;

    [SerializeField]
    private VideoPlayer videoPlayer = null;
    private VideoPlayer videoPlayer2;

    private VideoPlayer currVideoPlayer;

    [SerializeField]
    private Image blackBackground = null;

    [SerializeField]
    private Image deathBlackBackground = null;

    [SerializeField]
    private SO_TussleSettings settings = null;

    [SerializeField]
    private SO_DeveloperToolsSettings devTools = null;

    [SerializeField]
    private PSO_CurrentTussle currTussle = null;

    [SerializeField]
    private BoolDelegateSO tussleResolveDebugDelegate = null;

    [SerializeField]
    private BoolDelegateSO playerDeathTrigger = null;

    [SerializeField]
    private SO_DamageQuantSettings damageSettings = null;

    [SerializeField]
    private PSO_CurrentEnergy currEnergy = null;

    [SerializeField]
    private PSO_FillBarQuant grappleCoolDown = null;

    private bool playerAdv => currTussle.Value.PlayerAdvantage;
    private bool bossTussle => currTussle.Value.BossTussle;

    private AudioWrapperSource audioSource;

    private TussleInputSequence currSequence;

    private Coroutine startCR = null;
    private Coroutine loopCR = null;
    private Coroutine waitForCurrVideoAndPlayCR = null;

    private bool gamePaused => currGameMode.Value == GameModeEnum.PAUSE;

    private void Start()
    {
        audioSource = GetComponent<AudioWrapperSource>();
        currTussle.RegisterForPropertyChanged(InitiateTussle);
        tussleResolveDebugDelegate.RegisterForDelegateInvoked(ResolveDebug);
        videoPlayer.targetCamera = Camera.main;
        videoPlayer.clip = null;

        //make the duplicate que videoplayer one at runtime to have the same settings
        videoPlayer2 = Instantiate(videoPlayer, transform);

        blackBackground.color = new Color(0, 0, 0, 0);
        deathBlackBackground.color = new Color(0, 0, 0, 0);
    }

    private void InitiateTussle(TussleWrapper _, TussleWrapper __)
    {
        blackBackground.color = new Color(0, 0, 0, 1);

        currGameMode.RegisterForPropertyChanged(OnGameModeChange);

        videoPlayer.enabled = true;
        videoPlayer2.enabled = true;

        treadmillSpeedDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(0, 0));
        energyBarDisplayDelegate.InvokeDelegateMethod(false);

        Time.timeScale = 0;

        TussleVideoWrapper startVidWrapper = settings.GetTussleVideoWrapper(playerAdv ? TussleVideoType.ADV_START : TussleVideoType.DIS_START);
        TussleVideoWrapper loopVidWrapper = settings.GetTussleVideoWrapper(playerAdv ? TussleVideoType.ADV_LOOP : TussleVideoType.DIS_LOOP);

        void startVideoLoadedCallback() => settings.TussleHazardCleanUpDelegate.InvokeDelegateMethod(true);

        startCR = StartCoroutine(PlayVideo(startVidWrapper, startVideoLoadedCallback));
        loopCR = WaitForCurrVideoAndPlay(loopVidWrapper);//StartCoroutine(WaitForCurrVideoAndPlay(loopVidWrapper));

        InitiateButtonSequence();
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

        yield return WaitForVideoToLoad(player);

        //TODO: putting this after wait for video to load solved a bug
        //of not playing the video at all. Can we not set the clips
        //same frame without doing .Prepare() ?
        player.Play();

        vidWrapper.AudioWrapper?.PlayAudioWrapper(audioSource);

        if (vidWrapper.TakeDamageAfterDelay)
        {
            StartCoroutine(TakeDamageCR(vidWrapper.DamageDelay));
        }

        if (vidWrapper.AlienDamageAfterDelay)
        {
            StartCoroutine(AlienDamageCR(vidWrapper.DamageDelay));
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
            currLives.ModifyValue(
                -1 * damageSettings.GetTussleDamage(damage2PlayerOrAlien: true));
        }
    }

    private IEnumerator AlienDamageCR(float delay)
    {
        yield return TussleWaitForSeconds(delay);
        bossTussleDamageDSO.InvokeDelegateMethod(bossTussle);
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
        while (currVideoPlayer == null || currVideoPlayer.clip == null || !currVideoPlayer.isPrepared || currVideoPlayer.isPlaying || gamePaused)
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

    private Coroutine WaitForCurrVideoAndPlay(TussleVideoWrapper queuedWrapper)
    {
        SafeStartCoroutine(ref waitForCurrVideoAndPlayCR, WaitForCurrVideoAndPlayCR(queuedWrapper), this);
        return waitForCurrVideoAndPlayCR;
    }

    private IEnumerator WaitForCurrVideoAndPlayCR(TussleVideoWrapper queuedWrapper)
    {
        VideoPlayer queuedPlayer = QueueTussleVideo(queuedWrapper);

        yield return WaitForVideoToLoad(queuedPlayer);
        yield return WaitForVideoToLoad(currVideoPlayer);
        yield return WaitForCurrVideoToFinish();

        //TODO: why do we flicker if after wait here?
        //In play video we need the play afterwards or else video no loads,
        //so why the flicker?
        queuedPlayer.Play();

        yield return WaitForVideoToLoad(queuedPlayer);

        //TODO: make a method for these two things together
        queuedWrapper.AudioWrapper?.PlayAudioWrapper(audioSource);

        if (queuedWrapper.TakeDamageAfterDelay)
        {
            StartCoroutine(TakeDamageCR(queuedWrapper.DamageDelay));
        }

        currVideoPlayer.clip = null;
        currVideoPlayer = queuedPlayer;

        waitForCurrVideoAndPlayCR = null;
    }

    private void InitiateButtonSequence()
    {
        StartCoroutine(InitiateButtonSequenceCR());
    }

    private IEnumerator InitiateButtonSequenceCR()
    {
        yield return TussleWaitForSeconds(settings.ShowSequenceDelay);

        currSequence = Instantiate(settings.GetInputSequencePrefab(playerAdv), transform);
        currSequence.StartSequence(SequenceResolved);
    }

    private void SequenceResolved(bool successful)
    {
        TussleVideoWrapper wrapper = successful ?
            settings.GetTussleVideoWrapper(playerAdv ? TussleVideoType.ADV_WIN : TussleVideoType.DIS_WIN)
            : settings.GetTussleVideoWrapper(playerAdv ? TussleVideoType.ADV_LOSE : TussleVideoType.DIS_LOSE);

        SafeStopCoroutine(ref waitForCurrVideoAndPlayCR, this);

        StartCoroutine(PlayVideo(wrapper, () => EndTussle(successful)));
    }

    private int ResolveDebug(bool successful)
    {
        SafeStopCoroutine(ref startCR, this);
        SafeStopCoroutine(ref loopCR, this);
        currSequence?.ResolveSequence(successful);
        return 0;
    }

    public void EndTussle(bool successful)
    {
        currGameMode.DeRegisterForPropertyChanged(OnGameModeChange);
        Time.timeScale = 1;

        Destroy(currSequence.gameObject);

        if (currLives.Value <= currLives.MinValue())
        {
            deathBlackBackground.color = new Color(0, 0, 0, 1);
            playerDeathTrigger.InvokeDelegateMethod(true);
            return;
        }

        currVideoPlayer = null;
        videoPlayer.clip = null;
        videoPlayer2.clip = null;

        blackBackground.color = new Color(0, 0, 0, 0);

        treadmillSpeedDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(1, 0));
        energyBarDisplayDelegate.InvokeDelegateMethod(true);

        currAction.ModifyValue(PlayerActionEnum.RUN);
        
        if (playerAdv)
        {
            if (successful)
            {
                currEnergy.RewardPlayerEnergy(PlayerActionEnum.TUSSLE);
            }

            //In case there is more than one enemy incoming and need to grapple fast
            grappleCoolDown.ModifyValue(new FillBarQuant(0, false, 0.001f));
        }
        
    }
}
