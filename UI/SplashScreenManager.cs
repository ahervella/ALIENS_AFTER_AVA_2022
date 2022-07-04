using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreenManager : MonoBehaviour
{
    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private SO_SplashScreenSettings settings = null;

    [SerializeField]
    private Image splashImg = null;

    private void Awake()
    {
        StartCoroutine(SplashScreenSequenceCR());
    }

    private IEnumerator SplashScreenSequenceCR()
    {
        splashImg.preserveAspect = true;
        
        foreach(SplashScreenWrapper ssw in settings.GetOrderedSplashScreenWrappers())
        {
            if (ssw.SplashImage == null)
            {
                splashImg.color = Color.black;
            }
            else
            {
                splashImg.sprite = ssw.SplashImage;
                splashImg.color = Color.white;
            }
            
            yield return new WaitForSeconds(ssw.Duration);
        }

        currGameMode.ModifyValue(GameModeEnum.MAINMENU);
    }
}
