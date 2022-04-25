using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class AlienBossDamageFlash : MonoBehaviour
{

    [SerializeField]
    private SpriteRenderer bossSprite = null;


    [SerializeField]
    private float hurtFlashWhiteLoopTime = 1f;

    private Coroutine flashDamageCR = null;
    private Color ogColor;

    private SpriteRenderer flashSprite;

    private void Awake()
    {
        ogColor = bossSprite.color;

        flashSprite = GetComponent<SpriteRenderer>();

        ogColor = new Color(1, 1, 1, 0);
        flashSprite.color = ogColor;
    }

    public void FlashWhiteDamage()
    {
        SafeStartCoroutine(ref flashDamageCR, FlashWhiteDamageCR(), this);
    }

    private IEnumerator FlashWhiteDamageCR()
    {
        //in case we are hit multiple times very quickly
        Color currStartClr = flashSprite.color;

        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / (hurtFlashWhiteLoopTime / 2);

            flashSprite.color = Color.Lerp(
                currStartClr, Color.white, EasedPercent(perc));

            //hack to get animation
            flashSprite.sprite = bossSprite.sprite;
            yield return null;
        }

        while (perc > 0)
        {
            perc -= Time.deltaTime / (hurtFlashWhiteLoopTime / 2);

            flashSprite.color = Color.Lerp(
                ogColor, Color.white, EasedPercent(perc));

            flashSprite.sprite = bossSprite.sprite;
            yield return null;
        }

        flashDamageCR = null;
    }
}
