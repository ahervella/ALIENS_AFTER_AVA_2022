using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using System;
using Random = UnityEngine.Random;
using PowerTools;

public class Boss3CannonDrone : MonoBehaviour
{
    [SerializeField]
    private SO_Boss3Settings settings = null;

    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    [SerializeField]
    private Vector3PropertySO hitBoxDimEdgePerc = null;

    [SerializeField]
    private Transform shootSpawnRef = null;

    [SerializeField]
    private int cannonLaneIndex = 0;
    public int CannonLaneIndex => cannonLaneIndex;

    [SerializeField]
    private Transform bossLaneChangeManagerTrans;

    [SerializeField]
    private int rawHeightPos = default;

    [SerializeField]
    private float rawEndDeathHeightPos = default;

    [SerializeField]
    private float deathRotQuant = 180f;

    [SerializeField]
    private BoxColliderSP hb = null;
    public BoxColliderSP HitBox => hb;

    [SerializeField]
    private int tileWidth = 1;

    [SerializeField]
    private SpriteRenderer sprite = null; 

    [SerializeField]
    private SpriteAnim disabledSpriteAnim = null;

    [SerializeField]
    private AnimationClip disabledAnim = null;

    [SerializeField]
    private Color disabledColor = default;
    private Color ogColor;

    [SerializeField]
    private float disabledTime = 1f;

    [SerializeField]
    private float restoreColorTransTime = 1f;

    private Coroutine restoreCR = null;

    private bool cannonDisabled => hb.BoxDisabled;

    private Shooter shooterInstance = null;

    private float cachedCannonLaneIndexPos;

    private float cachedCenterLanePos;

    private Vector3 cachedTarget;

    private void Awake()
    {
        cachedCannonLaneIndexPos = GetLaneXPosition(cannonLaneIndex, terrSettings);
        cachedCenterLanePos = GetLaneXPosition(0, terrSettings);

        disabledSpriteAnim.gameObject.SetActive(false);
        ogColor = sprite.color;

        SetHitBoxDimensionsAndPos(hb, new Vector2Int(tileWidth, 1), 1, terrSettings, hitBoxDimEdgePerc);

        hb.SetOnTriggerEnterMethod(
            coll => OnTriggerEnterCannonHitBox(coll, hb));
    }
    
    private void OnTriggerEnterCannonHitBox(Collider other, BoxColliderSP hbSource)
    {
        BoxColliderSP hb = other.gameObject.GetComponent<BoxColliderSP>();
        if (hb?.RootParent == hbSource) { return; }

        Projectile projectile = hb?.RootParent?.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.OnEnterBoss3CannonDrone(this);
            return;
        }
    }

    public void UpdateLaneIndex(int offsetFromOG)
    {
        cachedCannonLaneIndexPos = GetLaneXPosition(cannonLaneIndex + offsetFromOG, terrSettings);
    }

    public void TempDisableCannon()
    {
        disabledSpriteAnim.gameObject.SetActive(true);
        disabledSpriteAnim.Play(disabledAnim);
        sprite.color = disabledColor;
        hb.BoxDisabled = true;

        SafeStartCoroutine(ref restoreCR, RestoreCannonCR(), this);
    }

    private IEnumerator RestoreCannonCR()
    {
        yield return new WaitForSeconds(disabledTime);
        hb.BoxDisabled = false;
        disabledSpriteAnim.gameObject.SetActive(false);

        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / restoreColorTransTime;
            Color clr = Color.Lerp(disabledColor, ogColor, perc);
            sprite.color = clr;
            yield return null;
        }
    }

    private void Update()
    {
        cachedTarget = new Vector3(
            cachedCannonLaneIndexPos + bossLaneChangeManagerTrans.position.x - cachedCenterLanePos,
            rawHeightPos,
            1.5f * terrSettings.TileDims.y
        );

        shootSpawnRef.LookAt(cachedTarget);
    }

    public void InstanceShooter(bool rage, int shotCount = 1)
    {
        if (cannonDisabled) { return; }

        ShooterWrapper sw = settings.BeamShooterWrapper(rage);
        Vector3 projectilePos() => shootSpawnRef.position;

        shooterInstance = Shooter.InstantiateShooterObject(
            shootSpawnRef,
            shootSpawnRef,
            projectilePos,
            hb,
            sw,
            shotCount);
        shooterInstance.transform.localPosition = Vector3.zero;
        shooterInstance.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void CleanUpShooter()
    {
        if (shooterInstance == null) { return; }
        SafeDestroy(shooterInstance.gameObject);
    }

    public void InitDeath()
    {
        CleanUpShooter();
        StartCoroutine(DeathSpinFallCR());
    }

    private IEnumerator DeathSpinFallCR()
    {
        yield return new WaitForSeconds(Random.value * settings.DroneDeathFallRandDelayRange);

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, deathRotQuant);

        float startYPos = transform.localPosition.y;

        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / settings.DeathFallTime;

            float yPos = Mathf.Lerp(startYPos, rawEndDeathHeightPos, EasedPercent(perc));
            transform.localPosition = new Vector3(
                transform.localPosition.x, yPos, transform.localPosition.z);

            transform.localRotation = Quaternion.Lerp(startRot, endRot, EasedPercent(perc));

            yield return null;
        }

        SafeDestroy(gameObject);
    }
}
