using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HelperUtil;

[RequireComponent(typeof(RectTransform))]
public abstract class AFillBarManager<PSO_CURR_QUANT, FILL_BAR_SETTINGS> : MonoBehaviour where PSO_CURR_QUANT : IntPropertySO where FILL_BAR_SETTINGS : SO_AFillBarSettings
{
    [SerializeField]
    protected FILL_BAR_SETTINGS settings = null;

    [SerializeField]
    private Image maskImg = null;

    [SerializeField]
    private Image frameImg = null;

    [SerializeField]
    private Image maskFillImg = null;

    [SerializeField]
    protected PSO_CURR_QUANT currQuant = null;

    [SerializeField]
    private RectTransform barDisplayContainer = null;


    /// <summary>
    /// A fraction of an energy block in bar percent that is meant
    /// to give the illusion of a smooth bar filling even though
    /// it only operates in integers
    /// </summary>
    private float blockFractionPerc;

    /// <summary>
    /// Target number of blocks in bar percent
    /// </summary>
    private float targetFillAmount;

    /// <summary>
    /// Previous fill amount (bar percent) at the time of energy change
    /// </summary>
    private float prevFillAmount;

    /// <summary>
    /// The current tween percent (0 = start, 1 = complete)
    /// </summary>
    private float currTweenPerc;

    /// <summary>
    /// Defines the amount of auto delta (negative or positive) per
    /// second, if any at all
    /// </summary>
    protected float autoDeltaRatePerSec = 0;

    private Vector3 finalSpawnPos;

    private bool spawnBarFillAnimInProgress = false;

    private void Awake()
    {
        //energyBarDisplayDelegate.RegisterForDelegateInvoked(SetVisibility);
        //currEnergy.RegisterForPropertyChanged(OnEnergyChanged);
        //currAction.RegisterForPropertyChanged(OnActionChanged);
        currQuant.RegisterForPropertyChanged(OnCurrQuantChanged);
        blockFractionPerc = 0;
        AfterAwake();
    }

    protected abstract void AfterAwake();

    private void OnCurrQuantChanged(int oldQuant, int newQuant)
    {

        targetFillAmount = currQuant.Value / (float)settings.MaxQuant;

        //minus fractionFillPerc so that it's not part of the interpolated amount
        //also need to use targeFill for when doing the spaw anim so we
        //don't jump around the bar if we trigger this mid spawn anim
        prevFillAmount = (spawnBarFillAnimInProgress ?
            targetFillAmount : maskImg.fillAmount)
            - blockFractionPerc;
        currTweenPerc = 0;
        AfterModifyCurrQuant(oldQuant, newQuant);
    }

    protected abstract void AfterModifyCurrQuant(int oldQuant, int newQuant);

    private void Start()
    {
        SetStartingEnergy();
        StartSpawnAnimations();
        AfterStart();
    }

    private void SetStartingEnergy()
    {
        currQuant.ModifyValue(settings.StartingQuant);
        currTweenPerc = 1;
    }

    private void StartSpawnAnimations()
    {
        if (settings.FadeIn)
        {
            StartCoroutine(SpawnFadeIn());
        }

        if (settings.SpawnFromTop)
        {
            StartCoroutine(MoveBarToFinalSpawnPos());
        }

        if (settings.AnimateBarFillOnSpawn)
        {
            StartCoroutine(AnimateBarFill());
        }
    }

    private IEnumerator SpawnFadeIn()
    {
        Color ogColor = maskImg.color;
        maskFillImg.color = new Color(ogColor.r, ogColor.g, ogColor.b, 0);
        frameImg.color = new Color(ogColor.r, ogColor.g, ogColor.b, 0);

        yield return new WaitForSeconds(settings.FadeInDelay);

        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / settings.FadeInTime;
            maskFillImg.color = new Color(ogColor.r, ogColor.g, ogColor.b, EasedPercent(perc));
            frameImg.color = new Color(ogColor.r, ogColor.g, ogColor.b, EasedPercent(perc));
            yield return null;
        }
    }

    private IEnumerator MoveBarToFinalSpawnPos()
    {
        finalSpawnPos = barDisplayContainer.transform.position;
        float barHeight = barDisplayContainer.rect.height;
        float screenHeight = GetComponent<RectTransform>().rect.height;

        //assuming anchor is top center
        float tweenStartYPos = barHeight / 2f + screenHeight;

        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / settings.SpawnFromTopTime;

            float yPos = Mathf.Lerp(tweenStartYPos, finalSpawnPos.y, EasedPercent(perc));

            barDisplayContainer.position = new Vector3(
                barDisplayContainer.position.x,
                yPos,
                barDisplayContainer.position.z);

            yield return null;
        }

        transform.position = finalSpawnPos;
    }

    private IEnumerator AnimateBarFill()
    {
        spawnBarFillAnimInProgress = true;
        maskImg.fillAmount = 0;

        float delay = settings.BarFillOnSpawnDelay
                + (settings.SpawnFromTop ? settings.SpawnFromTopTime : 0);

        yield return new WaitForSeconds(delay);

        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / settings.BarFillOnSpawnTime;
            maskImg.fillAmount = EasedPercent(perc) * targetFillAmount;
            Debug.Log("maskImg fillamount: " + maskImg.fillAmount);
            yield return null;
        }

        spawnBarFillAnimInProgress = false;
    }

    protected abstract void AfterStart();

    protected int SetVisibility(bool visibility)
    {
        barDisplayContainer.transform.gameObject.SetActive(visibility);
        return 0;
    }

    private void Update()
    {
        if (spawnBarFillAnimInProgress) { return; }
        BarTweenTick();
        RechargeTick();
    }

    private void BarTweenTick()
    {
        if (currTweenPerc >= 1) { return; }

        currTweenPerc += Time.deltaTime;

        maskImg.fillAmount =
            blockFractionPerc
            + prevFillAmount
            + (targetFillAmount - prevFillAmount)
            * EasedPercent(currTweenPerc);
    }

    private void RechargeTick()
    {
        //If there is no tween, then set it directly here
        if (currTweenPerc >= 1)
        {
            maskImg.fillAmount = targetFillAmount + blockFractionPerc;
        }

        blockFractionPerc += autoDeltaRatePerSec * Time.deltaTime / settings.MaxQuant;


        //Once we've passed the integer threshold, trigger the curr quant change
        float blockPerc = blockFractionPerc * settings.MaxQuant;

        if (blockPerc >= 1f || blockPerc <= 0f)
        {
            //in case a single auto delta step was more than 1 block
            int delta = (int)Mathf.Floor(blockPerc);
            blockPerc -= delta;
            blockFractionPerc = blockPerc / settings.MaxQuant;
            currQuant.ModifyValue(delta);
        }
    }
}
