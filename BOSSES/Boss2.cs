using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using System;
using Random = UnityEngine.Random;

public class Boss2 : AAlienBoss<Boss2State, SO_Boss2Settings>
{
    [SerializeField]
    private HitBoxWrapper centerHBW = null;

    [SerializeField]
    private HitBoxWrapper leftHBW = null;

    [SerializeField]
    private HitBoxWrapper rightHBW = null;

    [SerializeField]
    private float attackYPos = 1f;

    [SerializeField]
    private PSO_TerrainTreadmillNodes terrNodes = null;

    private Vector3 cachedSpawnPos;
    private Vector3 cachedFarDefaultPos;

    private Coroutine idleFlybyCR = null;
    private Coroutine flyOverCR = null;
    private Coroutine startAttackCR = null;
    private Coroutine openAttackWingsCR = null;

    //need this because stopping a nested coroutine
    //doesn't seem to work
    private bool deathFlag = false;

    private Vector3 currVel;

    protected override void InitHitBoxes()
    {
        List<HitBoxWrapper> customHitBoxWrappers = new List<HitBoxWrapper>()
        {
            centerHBW,
            leftHBW,
            rightHBW
        };

        MakeCustomHitBoxes(
                HitBox(),
                //TODO: move this out of settings and serialize it
                new Vector2Int(settings.HitBoxTileWidth, 1),
                hitBoxHeight,
                terrSettings,
                hitBoxDimEdgePerc,
                customHitBoxWrappers);

        centerHBW.InstancedHB.SetAsNodeHitBox(true);
        centerHBW.InstancedHB.SetOnTriggerEnterMethod(
            coll => OnTriggerEnterBossHitBox(coll, centerHBW.InstancedHB, tussleOnAttack: true));

        leftHBW.InstancedHB.SetOnTriggerEnterMethod(
            coll => OnTriggerEnterBossHitBox(coll, leftHBW.InstancedHB, tussleOnAttack: false));

        rightHBW.InstancedHB.SetOnTriggerEnterMethod(
            coll => OnTriggerEnterBossHitBox(coll, rightHBW.InstancedHB, tussleOnAttack: false));
    }

    protected override void InitDeath()
    {
        deathFlag = true;
        SafeStopCoroutine(ref idleFlybyCR, this);
        SafeStopCoroutine(ref flyOverCR, this);
        SafeStopCoroutine(ref startAttackCR, this);
        SafeStopCoroutine(ref openAttackWingsCR, this);

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = new Vector3(
            currVel.x * settings.DeathRiseTime + startPos.x,
            cachedSpawnPos.y + settings.SpawnFlyOverDipFloorHeight * terrSettings.FloorHeight,
            currVel.z * settings.DeathRiseTime + startPos.z);

        float perc = 0f;

        while (perc < 1f)
        {
            perc += Time.deltaTime / settings.DeathRiseTime;

            Vector3 xzPos = Vector3.Lerp(startPos, endPos, perc);
            float yPos = Mathf.Lerp(startPos.y, endPos.y, EasedPercent(perc));
            transform.localPosition = new Vector3(xzPos.x, yPos, xzPos.z);
            yield return null;
        }

        StartCoroutine(FallDeathAnim());
    }

    private IEnumerator FallDeathAnim()
    {
        yield return null;
    }

    protected override void InitRage()
    {
    }

    protected override void OnBossAwake()
    {
        SpawnSequence();
        currState.RegisterForPropertyChanged(OnBossStateChange);
    }

    private void OnBossStateChange(Boss2State oldState, Boss2State newState)
    {
        leftHBW.InstancedHB.BoxDisabled = true;
        rightHBW.InstancedHB.BoxDisabled = true;

        switch (newState)
        {
            case Boss2State.SPREAD_WINGS:
            case Boss2State.SPREAD_WINGS_MIDDLE_HIGH:
                leftHBW.InstancedHB.SetReqAvoidAction(PlayerActionEnum.ROLL);
                centerHBW.InstancedHB.SetReqAvoidAction(PlayerActionEnum.ROLL);
                rightHBW.InstancedHB.SetReqAvoidAction(PlayerActionEnum.ROLL);
                break;

            case Boss2State.SPREAD_WINGS_MIDDLE_LOW:
                leftHBW.InstancedHB.SetReqAvoidAction(PlayerActionEnum.JUMP);
                //centerHBW.InstancedHB.SetReqAvoidAction(PlayerActionEnum.NONE);
                rightHBW.InstancedHB.SetReqAvoidAction(PlayerActionEnum.JUMP);
                break;

            default:
                leftHBW.InstancedHB.BoxDisabled = false;
                rightHBW.InstancedHB.BoxDisabled = false;
                centerHBW.InstancedHB.SetReqAvoidAction(PlayerActionEnum.NONE);
                break;
                //TODO: finish up with left and right leaning wings?
                //case Boss2State.
        }
    }

    private void SpawnSequence()
    {
        SafeStartCoroutine(ref flyOverCR, FlyOverCR(), this);
    }

    private IEnumerator FlyOverCR()
    {
        yield return new WaitForSeconds(settings.SpawnDelay);

        float finalZPos = settings.SpawnTileRowsAway * terrSettings.TileDims.y;
        float zTargetDelta = finalZPos - cachedSpawnPos.z;
        float flyOverDip = settings.SpawnFlyOverDipFloorHeight * terrSettings.FloorHeight;

        cachedFarDefaultPos = cachedSpawnPos + new Vector3(0, 0, zTargetDelta);

        
        yield return DipFly(
            cachedSpawnPos,
            new Vector3(0, 0, zTargetDelta),
            new Vector3(0, flyOverDip, 0),
            settings.FlyOverTime,
            () => currState.ModifyValue(Boss2State.FLYOVER_RISE));


        invincible = false;
        SafeStartCoroutine(ref idleFlybyCR, IdleFlybySequenceCR(), this);
    }

    private IEnumerator DipFly(
        Vector3 basePos,
        Vector3 posDeltaTarget,
        Vector3 maxDipDelta,
        float flyTime,
        Action onDipApex = null)
    {
        float perc = 0;
        float dipPerc;
        bool oneShotApex = true;

        Vector3 prevPos = basePos + maxDipDelta;

        while (perc < 1)
        {
            if (deathFlag) { yield break; }

            perc += Time.deltaTime / flyTime;
            dipPerc = perc < 0.5 ? perc * 2 : 2 - perc * 2;

            Vector3 posDelta = Vector3.Lerp(Vector3.zero, posDeltaTarget, perc);
            Vector3 dipDelta = Vector3.Lerp(Vector3.zero, maxDipDelta, EasedPercent(dipPerc));


            transform.localPosition = basePos + posDelta + maxDipDelta - dipDelta;

            CacheCurrVel(ref prevPos);

            if (oneShotApex && perc > 0.5f)
            {
                oneShotApex = false;
                onDipApex?.Invoke();
            }
            yield return null;
        }
    }

    private void CacheCurrVel(ref Vector3 prevPos)
    {
        currVel = (transform.localPosition - prevPos) / Time.deltaTime;
        prevPos = transform.localPosition;
    }

    protected override void SetStartingPosition()
    {
        float x = terrSettings.LaneCount / 2f * terrSettings.TileDims.x;
        float z = -terrSettings.TileDims.y;

        transform.localPosition = new Vector3(x, transform.localPosition.y, z);
        cachedSpawnPos = transform.localPosition;
    }

    private IEnumerator IdleFlybySequenceCR()
    {
        bool flyInLeftOrRight = Random.value > 0.5f;

        Boss2State idleState = flyInLeftOrRight ? Boss2State.IDLE_FLY_LEFT : Boss2State.IDLE_FLY_RIGHT;
        float sideMod = flyInLeftOrRight ? 1 : -1;

        currState.ModifyValue(idleState);

        float xStartPos = settings.FlybyStartTileDelta * terrSettings.TileDims.x * sideMod;
        float yStartPos = settings.FlybyDipFloorHeight * terrSettings.FloorHeight;

        Vector3 startPos = new Vector3(xStartPos, yStartPos, 0);

        int flyByCount = settings.GetRandFlyByCount(Rage);

        while (flyByCount > 0)
        {
            flyByCount--;


            if (flyByCount == 0)
            {
                StartAttack(startPos + cachedFarDefaultPos, flyInLeftOrRight);
                yield break;
            }

            yield return DipFly(
                           cachedFarDefaultPos + startPos,
                           new Vector3(-startPos.x * 2, 0, 0),
                           new Vector3(0, yStartPos, 0),
                           settings.FullFlybyTime.GetVal(Rage)
                           );

            
            startPos = new Vector3(-startPos.x, startPos.y, startPos.z);

            flyInLeftOrRight = !flyInLeftOrRight;

            idleState = flyInLeftOrRight ? Boss2State.IDLE_FLY_LEFT : Boss2State.IDLE_FLY_RIGHT;
            currState.ModifyValue(idleState);
        }

        
    }

    private void StartAttack(Vector3 absStartPos, bool fromLeftOrRight)
    {
        SafeStartCoroutine(ref startAttackCR, StartAttackCR(absStartPos, fromLeftOrRight), this);
    }

    private IEnumerator StartAttackCR(Vector3 absStartPos, bool fromLeftOrRight)
    {
        float perc = 0;
        bool oneShotApex = true;

        float endZPos = settings.AttackStartTileDist * terrSettings.TileDims.y;
        Vector3 endPos = new Vector3(cachedFarDefaultPos.x, attackYPos, endZPos);

        float speed = settings.AttackZTileSpeed.GetVal(Rage) * terrSettings.TileDims.y;
        float time = Mathf.Abs(endZPos - absStartPos.z) / speed;

        Vector3 prevPos = absStartPos;

        while (perc < 1)
        {
            perc += Time.deltaTime / time;

            Vector2 xyPos = Vector2.Lerp(absStartPos, endPos, EasedPercent(perc));
            float zPos = Mathf.Lerp(absStartPos.z, endPos.z, perc);

            if (oneShotApex && perc > 0.5f)
            {
                oneShotApex = false;
                Boss2State attackState = fromLeftOrRight ? Boss2State.ATTACK_START_LEFT : Boss2State.ATTACK_START_RIGHT;
                currState.ModifyValue(attackState);
            }

            transform.localPosition = new Vector3(xyPos.x, xyPos.y, zPos);

            CacheCurrVel(ref prevPos);

            yield return null;
        }

        OpenAttackWings(speed, fromLeftOrRight);
    }

    private void OpenAttackWings(float speed, bool fromLeftOrRight)
    {
        //activate correct hit box here, or just sub to state change?
        SafeStartCoroutine(ref openAttackWingsCR, OpenAttackWingsCR(speed, fromLeftOrRight), this);
    }

    private IEnumerator OpenAttackWingsCR(float speed, bool fromLeftOrRight)
    {
        TreadmillAttachment(true);

        Vector3 startPos = transform.localPosition;
        Vector3 endPos = new Vector3(cachedFarDefaultPos.x, attackYPos, -terrSettings.TileDims.y);

        //50% chance of second attack on rage
        if (Rage && Random.value > 0.5)
        {
            Boss2State wingSpreadState = Boss2State.SPREAD_WINGS;/*Random.value > 0.5f ?
            Boss2State.SPREAD_WINGS_MIDDLE_HIGH
            : Boss2State.SPREAD_WINGS_MIDDLE_LOW;*/
            currState.ModifyValue(wingSpreadState);
        }

        float time = Mathf.Abs(endPos.z - startPos.z) / speed;
        float perc = 0;
        Vector3 prevPos = startPos;

        while (perc < 1)
        {
            perc += Time.deltaTime / time;
            transform.localPosition = Vector3.Lerp(startPos, endPos, perc);
            yield return null;
        }

        CacheCurrVel(ref prevPos);

        yield return new WaitForSeconds(settings.PosAttack2IdleDelay);

        TreadmillAttachment(false);

        StartCoroutine(IdleFlybySequenceCR());
    }

    private void TreadmillAttachment(bool attach)
    {
        if (attach)
        {
            terrNodes.Value.AttachTransform(transform, horizOrVert: false, useContainer: true);
            return;
        }

        terrNodes.Value.DettachTransform(transform, null, usedContainer: true);
    }
}
