using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerControls : MonoBehaviour
{
    const float SINGLE_MASH_TAP_TIME_CAP = 0.25f;
    const int TAPS_NEEDED_FOR_MASH = 3;

    Dictionary<int, TouchTracker> touchDict = new Dictionary<int, TouchTracker>();

    int mashKeyCounter = 0;
    float mashKeyTimer = 0f;

    TouchTracker.TAP_REGION mashTapRegion = TouchTracker.TAP_REGION.NONE;
    int mashTapCounter = 0;
    float mashTapTimer = 0f;

    public static event System.Action<InputData> OnInputAction = delegate { };

    public struct InputData
    {
        private Vector2 startPos;
        private Vector2 endPos;
        private RunnerGameObject.PLAYER_STATE state;

        public InputData(RunnerGameObject.PLAYER_STATE state, Vector2 startPos, Vector2 endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;
            this.state = state;
        }

        public InputData(RunnerGameObject.PLAYER_STATE state)
        {
            this.startPos = Vector2.zero;
            this.endPos = Vector2.zero;
            this.state = state;
        }

        public Vector2 getStartPos() { return startPos; }
        public Vector2 getEndPos() { return endPos; }
        public RunnerGameObject.PLAYER_STATE getState() { return state; }
    }

    class TouchTracker
    {
        public enum GESTURE { NONE, UP, DOWN, LEFT, RIGHT}
        public enum TAP_REGION { NONE, LEFT, RIGHT, CENTER }

        const float MAX_GESTURE_TIME = 0.75f;
        const float MIN_GESTURE_MOVEMENT = 50f;


        int id;
        //Touch touch;
        public Vector2 initPos;
        public Vector2 finalPos;

        float totalTime;
        Vector2 totalDist;

        bool finishedTouch = false;
        public GESTURE finalGesture = GESTURE.NONE;
        public TAP_REGION tapRegion = TAP_REGION.NONE;

        public TouchTracker(int id, Vector2 initPos)
        {
            this.id = id;
            this.initPos = initPos;
            float xRatio = initPos.x / Camera.current.scaledPixelWidth;

            if (xRatio > 0.66f)
            {
                tapRegion = TAP_REGION.RIGHT;
            }
            else if(xRatio > 0.33f)
            {
                tapRegion = TAP_REGION.CENTER;
            }
            else
            {
                tapRegion = TAP_REGION.LEFT;
            }
        }

        public void addTimeDist(float deltaTime, Vector2 newPos)
        {
            if (finishedTouch) { return; }

            totalTime += deltaTime;
            totalDist = newPos - initPos;
            finalPos = newPos;



            if (totalDist.magnitude >= MIN_GESTURE_MOVEMENT)
            {

                if (Mathf.Abs(totalDist.x) > Mathf.Abs(totalDist.y))
                {
                    finalGesture = totalDist.x > 0 ? GESTURE.RIGHT : GESTURE.LEFT;
                }
                else
                {
                    finalGesture = totalDist.y > 0 ? GESTURE.UP : GESTURE.DOWN;
                }

                
                finishedTouch = true;
                return;
            }


            if (totalTime > MAX_GESTURE_TIME) { finishedTouch = true; }

            //return GESTURE.NONE;
        }
    }



    private void Update()
    {
        //update mash tapping stuff if made it this far
        if (mashTapCounter > 0) {mashTapTimer += Time.deltaTime; }
        if (mashTapTimer >= SINGLE_MASH_TAP_TIME_CAP) { resetTapShit(); }

        if (mashKeyCounter > 0) { mashKeyTimer += Time.deltaTime; }
        if (mashKeyTimer >= SINGLE_MASH_TAP_TIME_CAP) { resetKeyShit(); }

        InputData touchInputData = getTouchInputData();
        if (touchInputData.getState() != RunnerGameObject.PLAYER_STATE.NONE)
        {
            OnInputAction(touchInputData);
        }

        InputData keyInputData = getKeyInputData();
        if (keyInputData.getState() != RunnerGameObject.PLAYER_STATE.NONE)
        {
            OnInputAction(keyInputData);
        }
    }

    private void resetKeyShit()
    {
        mashKeyTimer = 0; mashKeyCounter = 0;
    }

    private void resetTapShit()
    {
        mashTapTimer = 0; mashTapCounter = 0;
        mashTapRegion = TouchTracker.TAP_REGION.NONE;
    }


    private InputData getKeyInputData()
    {
        RunnerGameObject.PLAYER_STATE keyState = RunnerGameObject.PLAYER_STATE.NONE;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            mashKeyCounter++;
            mashKeyTimer = 0;

            if (mashKeyCounter >= TAPS_NEEDED_FOR_MASH)
            {
                resetKeyShit();
                return new InputData(RunnerGameObject.PLAYER_STATE.SPRINT);
            }
        }

        


        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            keyState = RunnerGameObject.PLAYER_STATE.DODGE_L;
        }

        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            keyState = RunnerGameObject.PLAYER_STATE.DODGE_R;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            keyState = RunnerGameObject.PLAYER_STATE.JUMP;
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            keyState = RunnerGameObject.PLAYER_STATE.ROLL;
        }

        else if (Input.GetKeyDown(KeyCode.R))
        {
            keyState = RunnerGameObject.PLAYER_STATE.THROW_R;
        }

        return new InputData(keyState);
    }


    private InputData getTouchInputData()
    {
        TouchTracker touchTracker = null;

        foreach (Touch touch in Input.touches)
        {
            

            if (!touchDict.ContainsKey(touch.fingerId) && touch.phase == TouchPhase.Began)
            {
                TouchTracker newTouch = new TouchTracker(touch.fingerId, touch.position);
                touchDict.Add(touch.fingerId, newTouch);

                if (mashTapRegion != TouchTracker.TAP_REGION.NONE && newTouch.tapRegion != mashTapRegion)
                {
                    resetTapShit();
                }
                else
                {
                    mashTapCounter++;
                    mashTapTimer = 0;
                }
                Debug.Log("Tapped!");
                Debug.Log(mashTapCounter);
                Debug.Log(mashTapTimer);

                mashTapRegion = newTouch.tapRegion;
                continue;
            }

            TouchTracker currTouchTracker = touchDict[touch.fingerId];

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Ended)
            {
                currTouchTracker.addTimeDist(Time.fixedDeltaTime, touch.position);
                touchDict.Remove(touch.fingerId);

                Debug.Log(touch.fingerId);
            }

            else
            {
                currTouchTracker.addTimeDist(Time.fixedDeltaTime, touch.position);
            }

            //cache the at least one touchTraker, or the last that isn't NONE
            if (touchTracker == null && currTouchTracker.finalGesture != TouchTracker.GESTURE.NONE) { touchTracker = currTouchTracker; }

        }


        //if there was a gesture, reset all mashTap tracking stuff
        if (touchTracker != null && touchTracker.finalGesture != TouchTracker.GESTURE.NONE) { resetTapShit(); }

        //if there were enough taps done correctly, its a sprint!
        if (mashTapCounter >= TAPS_NEEDED_FOR_MASH)
        {
            RunnerGameObject.PLAYER_STATE regionResultState = RunnerGameObject.PLAYER_STATE.SPRINT;
            switch (mashTapRegion)
            {
                case TouchTracker.TAP_REGION.CENTER:
                    break;

                case TouchTracker.TAP_REGION.LEFT:
                    regionResultState = RunnerGameObject.PLAYER_STATE.FIRE;
                    break;

                case TouchTracker.TAP_REGION.RIGHT:
                    regionResultState = RunnerGameObject.PLAYER_STATE.THROW_R;
                    break;
            }

            resetTapShit();
            if (touchTracker != null)
            {
                return new InputData(regionResultState, touchTracker.initPos, touchTracker.finalPos);
            }
        }

        if (touchTracker == null) { return new InputData(RunnerGameObject.PLAYER_STATE.NONE); }

        RunnerGameObject.PLAYER_STATE state = RunnerGameObject.PLAYER_STATE.NONE;

        switch (touchTracker.finalGesture)
        {
            case TouchTracker.GESTURE.UP:
                state = RunnerGameObject.PLAYER_STATE.JUMP;
                break;

            case TouchTracker.GESTURE.DOWN:
                state = RunnerGameObject.PLAYER_STATE.ROLL;
                break;

            case TouchTracker.GESTURE.LEFT:
                state = RunnerGameObject.PLAYER_STATE.DODGE_L;
                break;

            case TouchTracker.GESTURE.RIGHT:
                state = RunnerGameObject.PLAYER_STATE.DODGE_R;
                break;

            default:
                break;
        }

        return new InputData(state, touchTracker.initPos, touchTracker.finalPos);

        
    }
}

