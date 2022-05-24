using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HelperUtil;

//TODO: make the PSO quants modify every frame instead of faking it, and use the new ModifyNoInvoke to only trigger
//when we want to?
[RequireComponent(typeof(RectTransform))]
public abstract class AFillBarManager<PSO_CURR_QUANT, FILL_BAR_SETTINGS> : AFillBarManagerBase where PSO_CURR_QUANT : IntPropertySO where FILL_BAR_SETTINGS : SO_AFillBarSettings
{
    //TODO move non generic based args to base class  and set in base prefab
    [SerializeField]
    protected FILL_BAR_SETTINGS settings = null;

    [SerializeField]
    private IntPropertySO currZone = null;

    [SerializeField]
    private bool advanceZoneOnTearDown = false;

    [SerializeField]
    private Image maskImg = null;

    [SerializeField]
    protected Image frameImg = null;

    [SerializeField]
    protected Image maskFillImg = null;

    [SerializeField]
    private TextMeshProUGUI optionalLabel = null;

    [SerializeField]
    protected PSO_CURR_QUANT currQuant = null;

    [SerializeField]
    protected IntPropertySO optionalPSOMaxBarQuant = null;

    [SerializeField]
    private RectTransform barDisplayContainer = null;

    [SerializeField]
    private RectTransform spawnFromRef = null;

    [SerializeField]
    private bool NoTransitionOnDecrease = false;

    [SerializeField]
    private bool NoTransitionOnIncrease = false;

    [SerializeField]
    private BoolDelegateSO optionalFilledDelegate = null;


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

    protected int cachedMaxQuant = -1;

    protected Color startingFrameColor;

    protected Color startingMaskFillColor;

    private void Awake()
    {
        //energyBarDisplayDelegate.RegisterForDelegateInvoked(SetVisibility);
        //currEnergy.RegisterForPropertyChanged(OnEnergyChanged);
        //currAction.RegisterForPropertyChanged(OnActionChanged);
        currQuant.RegisterForPropertyChanged(OnCurrQuantChanged);
        blockFractionPerc = 0;
        startingFrameColor = frameImg.color;
        startingMaskFillColor = maskFillImg.color;
        AfterAwake();
    }

    protected abstract void AfterAwake();

    private void OnCurrQuantChanged(int oldQuant, int newQuant)
    {
        //Multiple bars share the energy PSO, so this is necessary for now
        MakeSureMaxQuantityCached();

        targetFillAmount = currQuant.Value / (float)cachedMaxQuant;

        if (optionalFilledDelegate != null && targetFillAmount == 1 && newQuant > oldQuant)
        {
            optionalFilledDelegate.InvokeDelegateMethod(true);
        }

        if ((NoTransitionOnDecrease && oldQuant > newQuant)
            || (NoTransitionOnIncrease && newQuant > oldQuant))
        {
            ImmediatelySetBarQuant();
        }
        else
        {
            //minus fractionFillPerc so that it's not part of the interpolated amount
            //also need to use targeFill for when doing the spaw anim so we
            //don't jump around the bar if we trigger this mid spawn anim
            prevFillAmount = (spawnBarFillAnimInProgress ?
                targetFillAmount : maskImg.fillAmount)
                - blockFractionPerc;
            currTweenPerc = 0;
        }

        AfterModifyCurrQuant(oldQuant, newQuant);
    }

    private void MakeSureMaxQuantityCached()
    {
        if (cachedMaxQuant == -1)
        {
            cachedMaxQuant = optionalPSOMaxBarQuant?.Value ?? settings.MaxQuant;
        }
    }

    private void ImmediatelySetBarQuant()
    {
        currTweenPerc = 1;
        SetFillAmount();
    }

    private void SetFillAmount()
    {
        maskImg.fillAmount =
               blockFractionPerc
               + prevFillAmount
               + (targetFillAmount - prevFillAmount)
               * EasedPercent(currTweenPerc);
    }

    protected abstract void AfterModifyCurrQuant(int oldQuant, int newQuant);

    private void Start()
    {
        MakeSureMaxQuantityCached();
        SetStartingQuant();
        StartSpawnAnimations();
        AfterStart();
    }

    private void SetStartingQuant()
    {
        if (!settings.SetStartingQuant) { return; }

        currQuant.ModifyValue(settings.StartingQuant - currQuant.Value);
        ImmediatelySetBarQuant();
    }


    private void StartSpawnAnimations()
    {
        if (settings.FadeIn)
        {
            StartCoroutine(SpawnFadeIn());
            StartCoroutine(LabelFadeIn());
        }

        if (settings.SpawnFromTop)
        {
            StartCoroutine(SpawnPositionTween());
        }

        if (settings.AnimateBarFillOnSpawn)
        {
            StartCoroutine(AnimateBarFill());
        }
    }

    private IEnumerator SpawnFadeIn(bool reverse = false)
    {
        Color ogColor = maskImg.color;
        float startingAlpha = reverse ? 1 : 0;
        maskFillImg.color = new Color(ogColor.r, ogColor.g, ogColor.b, startingAlpha);
        frameImg.color = new Color(ogColor.r, ogColor.g, ogColor.b, startingAlpha);

        float delay = reverse ?
            0//settings.SpawnFromTopTime - settings.FadeInDelay
            : settings.FadeInDelay;
        yield return new WaitForSeconds(delay);

        float perc = reverse ? 1 : 0;
        float dir = reverse ? -1 : 1;

        while (perc <= 1 && perc >= 0)
        {
            perc += Time.deltaTime / settings.FadeInTime * dir;
            maskFillImg.color = new Color(ogColor.r, ogColor.g, ogColor.b, EasedPercent(perc));
            frameImg.color = new Color(ogColor.r, ogColor.g, ogColor.b, EasedPercent(perc));
            yield return null;
        }
    }

    private IEnumerator LabelFadeIn(bool reverse = false)
    {
        if (optionalLabel == null) { yield break; }

        Color ogColor = optionalLabel.color;
        float startingAlpha = reverse ? 1 : 0;
        optionalLabel.color = new Color(ogColor.r, ogColor.g, ogColor.b, startingAlpha);

        float delay = reverse ? 0 : settings.LabelFadeInDelay;
        yield return new WaitForSeconds(delay);

        float perc = 0;
        float dir = reverse ? -1 : 1;

        while (perc <= 1 && perc >= 0)
        {
            perc += Time.deltaTime / settings.FadeInTime * dir;
            optionalLabel.color = new Color(ogColor.r, ogColor.g, ogColor.b, EasedPercent(perc));
            yield return null;
        }
    }

    private IEnumerator MoveBarToFinalSpawnPos(bool reverse = false)
    {
        finalSpawnPos = barDisplayContainer.transform.position;
        float barHeight = barDisplayContainer.rect.height;
        float screenHeight = GetComponent<RectTransform>().rect.height;

        //assuming anchor is top center
        float tweenStartYPos = barHeight / 2f + screenHeight;

        float dir = reverse ? -1 : 1;

        float perc = reverse ? 1 : 0;
        while (perc <= 1 && perc >= 0)
        {
            perc += Time.deltaTime / settings.SpawnFromTopTime * dir;

            float yPos = Mathf.Lerp(tweenStartYPos, finalSpawnPos.y, EasedPercent(perc));

            barDisplayContainer.position = new Vector3(
                barDisplayContainer.position.x,
                yPos,
                barDisplayContainer.position.z);

            yield return null;
        }

        barDisplayContainer.transform.position = reverse ? barDisplayContainer.position = new Vector3(
                barDisplayContainer.position.x,
                tweenStartYPos,
                barDisplayContainer.position.z)
            : finalSpawnPos;

        
    }

    private IEnumerator SpawnPositionTween(bool reverse = false)
    {
        Vector3 finalPos = barDisplayContainer.position;
        float dir = reverse ? -1 : 1;

        float perc = reverse ? 1 : 0;
        while (perc <= 1 && perc >= 0)
        {
            perc += Time.deltaTime / settings.SpawnFromTopTime * dir;

            barDisplayContainer.position = Vector3.Lerp(spawnFromRef.position, finalPos, EasedPercent(perc));
            yield return null;
        }

        //TODO: find a better way to do this that doesn't require more coroutine refs?
        if (reverse)
        {
            SafeDestroy(gameObject);
        }
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
            //Debug.Log("maskImg fillamount: " + maskImg.fillAmount);
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

        SetFillAmount();
    }

    private void RechargeTick()
    {
        if (autoDeltaRatePerSec == 0) { return; }
        //If there is no tween, then set it directly here
        if (currTweenPerc >= 1)
        {
            SetFillAmount();
        }

        blockFractionPerc += autoDeltaRatePerSec * Time.deltaTime / cachedMaxQuant;


        //Once we've passed the integer threshold, trigger the curr quant change
        float blockPerc = blockFractionPerc * cachedMaxQuant;

        if (blockPerc >= 1f || blockPerc <= 0f)
        {
            //in case a single auto delta step was more than 1 block
            int delta = (int)Mathf.Floor(blockPerc);
            blockPerc -= delta;
            blockFractionPerc = blockPerc / cachedMaxQuant;
            currQuant.ModifyValue(delta);
        }
    }

    public override void TearDown(float delay)
    {
        StartCoroutine(TearDownDelayCR(delay));
    }

    public IEnumerator TearDownDelayCR(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(SpawnPositionTween(reverse: true));
        StartCoroutine(SpawnFadeIn(reverse: true));

        if (advanceZoneOnTearDown)
        {
            currZone.ModifyValue(1);
        }
    }
}
