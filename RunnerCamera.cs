using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RunnerCamera : MonoBehaviour
{
    public Transform playerRef;

    Color CLEAR_TINT = new Color(1, 1, 1, 0);
    Color RED_TINT = new Color(1, 0, 0, 0);

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
        RunnerPlayer.onAnimationStarted += animStart;
        RunnerPlayer.onAnimationEnded += animEnd;
        RunnerPlayer.changeCamRedTint += setFlashing;

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

    private void FixedUpdate()
    {
        float actualDelta = smoothSpeed;

        //spherical ease on starting and stopping anims
        if (deltaTimeTotal < SphericalEaseTime)
        {
            deltaTimeTotal += Time.deltaTime; //* actionSmoothMultiplyer;
            deltaTimeTotal = Mathf.Min(deltaTimeTotal, SphericalEaseTime);

            actualDelta = RunnerGameObject.easingFunction(deltaTimeTotal / SphericalEaseTime * Mathf.PI);
        }
        
        
        //because player position is const changing, rot is not
        transform.position = Vector3.Lerp(transform.position, targetPosOffset + playerRef.position, actualDelta);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotOffset), actualDelta);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOVOffset, actualDelta);


        flashRedTint();
    }



    private void animStart(RunnerPlayer.PLAYER_STATE state)
    {
        offsetChange(state, true);

    }

    private void animEnd(RunnerPlayer.PLAYER_STATE state)
    {
        offsetChange(state, false);

    }

    private void offsetChange(RunnerPlayer.PLAYER_STATE state, bool startingAnim)
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


    void setFlashing(float loopDuration, int loopCount, bool isDead)
    {

        tintLoopTime = loopDuration * loopCount;
        tintLoopCount = loopCount;
        tintDeltaTimeTotal = 0f;
        tintIsDead = isDead;


    }
    

    void flashRedTint()
    {
        if (tintDeltaTimeTotal >= tintLoopTime) { return; }

        tintDeltaTimeTotal = Mathf.Min(tintLoopTime, tintDeltaTimeTotal + Time.fixedDeltaTime);

        float singleHalfLoopTime = tintLoopTime / (float)(tintLoopCount * 2);
        int halfLoopsDone = Mathf.FloorToInt(tintDeltaTimeTotal / singleHalfLoopTime);
        
        float delta = (tintDeltaTimeTotal - (singleHalfLoopTime * halfLoopsDone)) / singleHalfLoopTime;

        Color src = halfLoopsDone % 2 == 0 ? CLEAR_TINT : RED_TINT;
        Color end = halfLoopsDone % 2 == 0 ? RED_TINT : CLEAR_TINT;

        if (tintIsDead)
        {
            delta = tintDeltaTimeTotal / tintLoopTime;
            src = CLEAR_TINT;
            end = RED_TINT;

        }

        

        Color colVal = Color.Lerp(src, end, delta);

        camTintMat.SetColor("_Color", colVal);
    }
}
