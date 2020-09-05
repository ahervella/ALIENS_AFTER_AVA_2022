﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerControls : MonoBehaviour
{
    const float SINGLE_MASH_TAP_TIME_CAP = 0.25f;
    const int TAPS_NEEDED_FOR_MASH = 3;

    Dictionary<int, TouchTracker> touchDict = new Dictionary<int, TouchTracker>();

    int mashKeyCounter = 0;
    float mashKeyTimer = 0f;

    int mashTapCounter = 0;
    float mashTapTimer = 0f;

    public static event System.Action<TouchData> OnTouchAction = delegate { };

    public GestureDrawer gestureDrawer;

    public struct TouchData
    {
        Vector2 startPos;
        Vector2 endPos;
        RunnerGameObject.PLAYER_STATE state;
    }

    class TouchTracker
    {
        public enum GESTURE { NONE, UP, DOWN, LEFT, RIGHT}
        const float MAX_GESTURE_TIME = 0.75f;
        const float MIN_GESTURE_MOVEMENT = 50f;


        int id;
        //Touch touch;
        Vector2 initPos;
        float totalTime;
        Vector2 totalDist;

        bool finishedTouch = false;

        public TouchTracker(int id, Vector2 initPos)
        {
            this.id = id;
            this.initPos = initPos;
        }

        public GESTURE addTimeDist(float deltaTime, Vector2 newPos, GestureDrawer gestureDrawer)
        {
            if (finishedTouch) { return GESTURE.NONE; }

            totalTime += deltaTime;
            totalDist = newPos - initPos;

            

            if (totalDist.magnitude >= MIN_GESTURE_MOVEMENT)
            {
                gestureDrawer.renderShit(initPos, newPos);
                finishedTouch = true;

                if (Mathf.Abs(totalDist.x) > Mathf.Abs(totalDist.y))
                {
                    return totalDist.x > 0 ? GESTURE.RIGHT : GESTURE.LEFT;
                }
                else
                {
                    return totalDist.y > 0 ? GESTURE.UP : GESTURE.DOWN;
                }

                
            }

            if (totalTime > MAX_GESTURE_TIME) { finishedTouch = true; }

            return GESTURE.NONE;
        }
    }



    public RunnerGameObject.PLAYER_STATE getAction(float deltaTime)
    {
        //update mash tapping stuff if made it this far
        if (mashTapCounter > 0) {mashTapTimer += deltaTime; }
        if (mashKeyCounter > 0) { mashKeyTimer += deltaTime; }

        RunnerGameObject.PLAYER_STATE keyResult = getKeyAction();
        if (keyResult != RunnerGameObject.PLAYER_STATE.NONE) { return keyResult; }

        RunnerGameObject.PLAYER_STATE touchResult = getTouchAction();
        if (touchResult != RunnerGameObject.PLAYER_STATE.NONE) { return touchResult; }

        return RunnerGameObject.PLAYER_STATE.NONE;
    }



    RunnerGameObject.PLAYER_STATE getKeyAction()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mashKeyCounter++;
            mashKeyTimer = 0;

            if (mashKeyCounter >= TAPS_NEEDED_FOR_MASH)
            {
                mashKeyCounter = 0;
                mashKeyTimer = 0;
                return RunnerGameObject.PLAYER_STATE.SPRINT;
            }
        }

        if (mashKeyTimer >= SINGLE_MASH_TAP_TIME_CAP) { mashKeyTimer = 0; mashKeyCounter = 0; }


        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            return RunnerGameObject.PLAYER_STATE.DODGE_L;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            return RunnerGameObject.PLAYER_STATE.DODGE_R;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            return RunnerGameObject.PLAYER_STATE.JUMP;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            return RunnerGameObject.PLAYER_STATE.ROLL;
        }

        return RunnerGameObject.PLAYER_STATE.NONE;
    }


    RunnerGameObject.PLAYER_STATE getTouchAction()
    {
        TouchTracker.GESTURE result = TouchTracker.GESTURE.NONE;

        foreach (Touch touch in Input.touches)
        {
            TouchTracker.GESTURE prevResult = result;

            

            if (!touchDict.ContainsKey(touch.fingerId) && touch.phase == TouchPhase.Began)
            {
                mashTapCounter++;
                mashTapTimer = 0;
                touchDict.Add(touch.fingerId, new TouchTracker(touch.fingerId, touch.position));
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Ended)
            {
                result = touchDict[touch.fingerId].addTimeDist(Time.fixedDeltaTime, touch.position, gestureDrawer);
                touchDict.Remove(touch.fingerId);

                Debug.Log(touch.fingerId);
            }

            else
            {
                result = touchDict[touch.fingerId].addTimeDist(Time.fixedDeltaTime, touch.position, gestureDrawer);
            }

            //basically so that it gets the first gesture it finds of all the touches as the overall result
            if (prevResult != TouchTracker.GESTURE.NONE) { result = prevResult; }

        }

        //if there was a gesture, reset all mashTap tracking stuff
        if (result != TouchTracker.GESTURE.NONE) { mashTapCounter = 0; mashTapTimer = 0; }

        //if there were enough taps done correctly, its a sprint!
        if (mashTapCounter >= TAPS_NEEDED_FOR_MASH) { mashTapCounter = 0; return RunnerGameObject.PLAYER_STATE.SPRINT; }

        if (mashTapTimer >= SINGLE_MASH_TAP_TIME_CAP) { mashTapTimer = 0; mashTapCounter = 0; }



        switch (result)
        {
            case TouchTracker.GESTURE.UP:
                return RunnerGameObject.PLAYER_STATE.JUMP;

            case TouchTracker.GESTURE.DOWN:
                return RunnerGameObject.PLAYER_STATE.ROLL;

            case TouchTracker.GESTURE.LEFT:
                return RunnerGameObject.PLAYER_STATE.DODGE_L;

            case TouchTracker.GESTURE.RIGHT:
                return RunnerGameObject.PLAYER_STATE.DODGE_R;

            default:
                return RunnerGameObject.PLAYER_STATE.NONE;
        }

        
    }
}

