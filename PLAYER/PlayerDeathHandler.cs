using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioWrapperSource))]
public class PlayerDeathHandler : MonoBehaviour
{
    //TODO: make a delegate that takes in no params?
    [SerializeField]
    private BoolDelegateSO playerDeathTrigger = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private SO_PlayerDeathSettings settings = null;

    [SerializeField]
    private SO_DeveloperToolsSettings devToolsSettings = null;

    [SerializeField]
    private Image cut2BlackImg = null;

    private AudioWrapperSource audioSource = null;

    private void Awake()
    {
        cut2BlackImg.color = new Color(0, 0, 0, 0);
        audioSource = GetComponent<AudioWrapperSource>();
        playerDeathTrigger.SetInvokeMethod(OnDeath);
    }

    private int OnDeath(bool _)
    {
        settings.DeathAudio?.PlayAudioWrapper(audioSource);
        StartCoroutine(DeathCR());
        return 0;
    }

    private IEnumerator DeathCR()
    {
        float cut2BlackTime = devToolsSettings.SkipDeathSequence ? 0 : settings.DeathCut2BlackTime;
        float holdBlackTime = devToolsSettings.SkipDeathSequence ? 0 : settings.HoldBlackTime;

        yield return new WaitForSeconds(cut2BlackTime);
        cut2BlackImg.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(holdBlackTime);

        currGameMode.ModifyValue(GameModeEnum.MAINMENU);
    }
}
