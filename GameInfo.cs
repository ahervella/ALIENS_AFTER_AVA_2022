using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameInfo : Singleton<GameInfo>
{
    //Click on the region drop downs to expose the code!

    #region EDIT HERE TO ADD NEW TYPE AND DICTIONARY ENTRY
    //remember to make all caps, use underscore for space
    public enum INFO
    {
        NONE,
        LIVES,
        GUN_BULLETS
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

        { INFO.LIVES , new GameInfoTypeDetails(GetLives, new Dictionary<int, string>{
            { 4, "Health 4/4" },
            { 3, "Health 3/4" },
            { 2, "Health 2/4" },
            { 1, "Health 1/4" },
            { 0, "Dead" }
        }) },

        { INFO.GUN_BULLETS , new GameInfoTypeDetails(GetAmmo, new Dictionary<int, string>{
            { 3, "3 Rounds" },
            { 2, "2 Rounds" },
            { 1, "1 Round" },
            { 0, "Empty" },
        }) }
    };

    #endregion

    #region GAME INFO GETTER METHODS, ADD TO THIS REGION

    public static Func<int> GetGetterMethod(INFO info)
    {
        if (GameInfoDict.TryGetValue(info, out var details))
        {
            return details.GetState;
        }

        Debug.LogError("Could not find getter method for specific info type: " + info);
        return null;
    }

    /*
    EXAMPLE METHOD:
    private static int getMeAPropertyOrInfo()
    {
        .....
    }
    */

    public static int GetLives()
    {
        return RunnerPlayer.Lives;
    }

    public static int GetAmmo()
    {
        return RunnerPlayer.GunBullets;
    }

    #endregion


    #region DON'T TOUCH THIS SHIT

    protected override GameInfo GetSelf() => this;

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
