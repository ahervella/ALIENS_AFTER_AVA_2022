using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameInfo : MonoBehaviour
{
    //Click on the region drop downs to expose the code!

    #region EDIT HERE TO ADD NEW TYPE AND DICTIONARY ENTRY
    //remember to make all caps, use underscore for space
    public enum INFO
    {
        NONE,
        LIVES
    };


    private static Dictionary<INFO, GameInfoTypeDetails> GameInfoDict { get; set; } = new Dictionary<INFO, GameInfoTypeDetails>
    {
        /*
        EXAMPLE ENTRY:

        { INFO.[INFO_ENUM_YOU_JUST_ADDED] , new GameInfoTypeDetails(getterMethodYouJustAdded, new Dictionary<int, string>{
            { 1, "specific scenario A" },
            { 2, "specific scenario B" },
            { 3, "specific scenario C" }
        }) }
         */

        { INFO.LIVES , new GameInfoTypeDetails(getLives, new Dictionary<int, string>{
            { 4, "Health 4/4" },
            { 3, "Health 3/4" },
            { 2, "Health 2/4" },
            { 1, "Health 1/4" },
            { 0, "Dead" }
        }) }
    };

    #endregion

    #region GAME INFO GETTER METHODS, ADD TO THIS REGION

    /*
    EXAMPLE METHOD:
    private static int getMeAPropertyOrInfo()
    {
        .....
    }
    */

    private static int getLives()
    {
        return RunnerPlayer.Lives;
    }

    #endregion


    #region DON'T TOUCH THIS SHIT

    public static GameInfo Current = null;

    private void Awake()
    {
        if (Current == null)
        {
            Current = this;
        }
        else if (Current != this)
        {
            Destroy(gameObject);
        }
    }

    public class GameInfoTypeDetails
    {
        public Func<int> GetState { get; private set; }

        public Dictionary<int, string> StateNameDict { get; private set; }

        public GameInfoTypeDetails(Func<int> getState, Dictionary<int, string> stateNameDict)
        {
            this.GetState = getState;
            this.StateNameDict = stateNameDict;
        }
    }

    public static GameInfoTypeDetails GetGameInfoTypeDetails(INFO info)
    {
        if (GameInfoDict.TryGetValue(info, out var details))
        {
            return details;
        }

        return null;
    }
    #endregion

}
