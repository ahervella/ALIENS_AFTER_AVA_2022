using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RunnerCamera : MonoBehaviour
{
    public Transform playerRef;

    Color CLEAR_TINT = new Color(1, 1, 1, 0);
    Color RED_TINT = new Color(1, 0, 0, 0);

    [SerializeField]
    private IntPropertySO livesSO = null;

    [SerializeField]
    private List<CameraFlashSettings> flashSettings = null;

    public Material camTintMat;

    float tintDeltaTimeTotal = 0f;
    float tintLoopTime;
    int tintLoopCount = 1;
    bool tintIsDead = false;

    Camera cam;

    public float smoothSpeed;

    public float SphericalEaseTime;

    //Vector3(11.798996,0,0)
    public Vector3 startRotOffset;

    //Vector3(-0.569999993,1.38999999,-6.0999999)
    public Vector3 startPosOffset;

    public float startFOVOffset;


    public List<ActionOffset> actionOffset = new List<ActionOffset>();
    Dictionary<RunnerGameObject.PLAYER_STATE, ActionOffset> actionOffsetDict = new Dictionary<RunnerGameObject.PLAYER_STATE, ActionOffset>();

    Vector3 targetPosOffset;
    Vector3 targetRotOffset;
    public float targetFOVOffset;

    float deltaTimeTotal = 0f;

    [System.Serializable]
    public struct ActionOffset
    {
        public RunnerGameObject.PLAYER_STATE state;
        public Vector3 posOffset;
        public Vector3 rotOffset;
        public float FOVOffset;
    }

    private void Start()
    {
        livesSO.RegisterForPropertyChanged( OnLivesChanged );

        RunnerPlayer.OnAnimationStarted += AnimStart;
        RunnerPlayer.OnAnimationEnded += AnimEnd;

        cam = GetComponent<Camera>();

        targetPosOffset = startPosOffset;
        targetRotOffset = startRotOffset;
        targetFOVOffset = startFOVOffset;

        foreach (ActionOffset actionOS in actionOffset)
        {
            actionOffsetDict.Add(actionOS.state, actionOS);
        }

        camTintMat.SetColor("_Color", CLEAR_TINT);
    }

    void OnLivesChanged( int previous, int current )
    {
        int delta = current - previous;
        CameraFlashSettings settings = flashSettings.FirstOrDefault( x => x.deltaAmount == delta );
        if ( settings == null )
        {
            Debug.LogWarning( $"No camera flash settings found for a health delta of [{delta}]" );
            return;
        }
        Debug.Log($"Starting flash camera settings for a health delta of [{delta}] with settings of [{settings.ToString()}]");
        StopCoroutine( "FlashCoroutine" );
        StartCoroutine( "FlashCoroutine", settings );        
    }

    private void FixedUpdate()
    {
        float actualDelta = smoothSpeed;

        //spherical ease on starting and stopping anims
        if (deltaTimeTotal < SphericalEaseTime)
        {
            deltaTimeTotal += Time.deltaTime; //* actionSmoothMultiplyer;
            deltaTimeTotal = Mathf.Min(deltaTimeTotal, SphericalEaseTime);

            actualDelta = RunnerGameObject.EasingFunction(deltaTimeTotal / SphericalEaseTime * Mathf.PI);
        }


        //because player position is const changing, rot is not
        transform.position = Vector3.Lerp(transform.position, targetPosOffset + playerRef.position, actualDelta);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotOffset), actualDelta);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOVOffset, actualDelta);

    }



    private void AnimStart(RunnerPlayer.PLAYER_STATE state)
    {
        OffsetChange(state, true);

    }

    private void AnimEnd(RunnerPlayer.PLAYER_STATE state)
    {
        OffsetChange(state, false);

    }

    private void OffsetChange(RunnerPlayer.PLAYER_STATE state, bool startingAnim)
    {
        if (!actionOffsetDict.ContainsKey(state)) { return; }

        Vector3 posOffset = actionOffsetDict[state].posOffset;
        Vector3 rotOffset = actionOffsetDict[state].rotOffset;
        float FOVOffset = actionOffsetDict[state].FOVOffset;

        int applyMod = startingAnim ? 1 : 0;

        targetRotOffset = startRotOffset + rotOffset * applyMod;
        targetPosOffset = startPosOffset + posOffset * applyMod;
        targetFOVOffset = startFOVOffset + FOVOffset * applyMod;

        //apply spherical ease on starting and stopping anim;
        deltaTimeTotal = 0f;
    }

    IEnumerator FlashCoroutine( CameraFlashSettings settings )
    {
        float currentTime = 0;
        while ( currentTime < settings.loopCount * settings.loopDuration)
        {
            Debug.Log($"FlashCoroutine : [{currentTime}]");
            float singleHalfLoopTime = settings.loopDuration / 2;
            int halfLoopsDone = Mathf.FloorToInt(currentTime / singleHalfLoopTime);

            float delta = (currentTime - (singleHalfLoopTime * halfLoopsDone)) / singleHalfLoopTime;

            Color src = halfLoopsDone % 2 == 0 ? CLEAR_TINT : settings.flashColor;
            Color end = halfLoopsDone % 2 == 0 ? settings.flashColor : CLEAR_TINT;

            Color colVal = Color.Lerp(src, end, delta);

            camTintMat.SetColor("_Color", colVal);

            currentTime += Time.deltaTime;

            yield return null;
        }
    }

    [Serializable]
    public class CameraFlashSettings
    {
        [SerializeField]
        public float loopDuration = 1f;
        [SerializeField]
        public float loopCount = 1;
        [SerializeField]
        public Color flashColor = Color.red;
        [SerializeField]
        public int deltaAmount = -1;

        public override string ToString()
        {
            return $"duration : [{loopDuration}] - count : [{loopCount}]";
        }
    }
}

