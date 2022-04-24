using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HelperUtil;

public class RunTransitionManager : MonoBehaviour
{
    [SerializeField]
    private SO_RunTransitionSettings settings = null;

    [SerializeField]
    private Image blackScreen = null;

    private void Awake()
    {
        blackScreen.color = Color.black;
        StartCoroutine(FadeInCR());
    }

    private IEnumerator FadeInCR()
    {
        yield return new WaitForSeconds(settings.BlackHoldOnLoad);

        float perc = 1f;
        while (perc > 0)
        {
            perc -= Time.deltaTime / settings.FadeInTime;
            blackScreen.color = new Color(0, 0, 0, EasedPercent(perc));
            yield return null;
        }
    }
}
