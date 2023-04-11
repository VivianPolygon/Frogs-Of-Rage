using System.Collections.Generic;
using UnityEngine;
using System;

public static class LeaderboardData //used to manage and save out leaderboard time data
{
    #region "Private Variables/Functions"

    private static Dictionary<PlayerPath, List<LeaderboardScoreData>> _scores; 


    //tracks the scores data. also used for efficiency in the create leaderboard data function

    private static LeaderboardScoreData ConvertScoreAndNameToScoreData(float time, string name)
    {
        LeaderboardScoreData newScoreData;

        newScoreData.time = time;
        newScoreData.name = name;

        return newScoreData;
    }
    //Converts a list of times and names to a single score data. used in CreateLeaderboardData

    private static List<LeaderboardScoreData> SortScoreDataList(List<LeaderboardScoreData> scoreData)
    {
        scoreData = UtilityFunctions.SortScoreDataByLowestScore(scoreData);
        return scoreData;
    }
    //Sorts a list of score data lowest time first.

    private static List<LeaderboardScoreData> GetPathScores(PlayerPath playerPath)
    {
        if(_scores == null)
        {
            _scores = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
        }
        if(!_scores.TryGetValue(playerPath, out List<LeaderboardScoreData> scoreData))
        {
            scoreData = new List<LeaderboardScoreData>();
            _scores.Add(playerPath, scoreData);
        }

        return scoreData;
    }


    #endregion

    #region "Public Functions"
    /// <summary>
    /// Creates a sorted list of the score data. saves the sorted list on LeaderboardData and returns it.
    /// </summary>
    /// <param name="times"> list of times </param>
    /// <param name="names"> list of names </param>
    /// <returns></returns>
    public static List<LeaderboardScoreData> CreateLeaderboardData(List<float> times, List<string> names, PlayerPath playerPath)
    {
        if(times == null)
        {
            Debug.LogError("times list inputted into CreateLeaderboardData was null");
            return null;
        }
        if(names == null)
        {
            Debug.LogWarning("Names list inputed into CreateLeaderboardData was null");
            names = new List<string>();
        }

        times = UtilityFunctions.ClampListLength(times, 10);

        if (!_scores.TryGetValue(playerPath, out List<LeaderboardScoreData> scoreData))
        {
            scoreData = new List<LeaderboardScoreData>();
            _scores.Add(playerPath, scoreData);
        }


        List<LeaderboardScoreData> scoreDataList = new List<LeaderboardScoreData>();

        if (scoreData != null)
        {
            for (int i = 0; i < times.Count; i++)
            {
                LeaderboardScoreData currentData;
                if (scoreData.Count > i)
                {
                    currentData = scoreData[i];
                }
                else
                {
                    currentData = new LeaderboardScoreData();
                }

                currentData.time = times[i];
                if (names.Count > i)
                    currentData.name = names[i];
                else
                    currentData.name = "AAA";

                scoreDataList.Add(currentData);
            }
        }
        else
        {
            for (int i = 0; i < times.Count; i++)
            {
                LeaderboardScoreData currentData = new LeaderboardScoreData();
                currentData.time = times[i];
                if (names.Count > i)
                    currentData.name = names[i];
                else
                    currentData.name = "AAA";

                scoreDataList.Add(currentData);
            }
        }

        if(scoreDataList.Count > 1)
        {
            scoreDataList = UtilityFunctions.SortScoreDataByLowestScore(scoreDataList);
        }

        return scoreDataList;
    }
    //creates a set of up to 10 leader board data, utilizes _scores for efficnency, and sorts using SortScoreDataList. sets it to _scores to save it, returns it as well. takes a list of floats and strings to create the score data
    /// <summary>
    /// Creates a list of leaderboard data. takes in leaderboard data. useful for just sorting and clamping the list. to create leaderboard data out of floats and strings use the version of this function that takes in a float and a string
    /// </summary>
    /// <param name="leaderboardscoreData"> list of leaderboard data </param>
    /// <returns></returns>
    public static List<LeaderboardScoreData> CreateLeaderboardData(List<LeaderboardScoreData> leaderboardscoreData)
    {
        if (leaderboardscoreData == null)
        {
            Debug.LogError("score data list inputted was null");
            return null;
        }

        leaderboardscoreData = UtilityFunctions.ClampListLength(leaderboardscoreData, 10);

        if (leaderboardscoreData.Count > 1)
        {
            leaderboardscoreData = UtilityFunctions.SortScoreDataByLowestScore(leaderboardscoreData);
        }

        return leaderboardscoreData;
    }
    //does the same opperation of the float string counterpart, but without the conversions. basicaly just sorts and clamps whats inputed.

    /// <summary>
    /// Appends a single specified scoredata to the list. sorts and clamps after adding the data. saves the sorted list on LeaderboardData and returns it.
    /// </summary>
    /// <param name="time"> the time for the Score data</param>
    /// <param name="name"> the 3 letter name for the score data</param>
    /// <returns></returns>
    public static List<LeaderboardScoreData> AppendLeaderboardData(float time, string name, PlayerPath playerPath)
    {
        if(_scores == null)
        {
            _scores = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
        }

        LeaderboardScoreData newData;
        newData.time = time;
        newData.name = name;

        if(!_scores.TryGetValue(playerPath, out List<LeaderboardScoreData> scoreData))
        {
            scoreData = new List<LeaderboardScoreData>();
            _scores.Add(playerPath, scoreData);
        }

        scoreData.Add(newData);

        scoreData = UtilityFunctions.SortScoreDataByLowestScore(scoreData);
        scoreData = UtilityFunctions.ClampListLength(scoreData, 10);

        return scoreData;
    }
    //appends a single leaderboard data to the current scores. sorts and clams the list. sets it and returns it. takes a float and a string and creates a score data
    /// <summary>
    /// Appends a single specified scoredata to the list. sorts and clamps after adding the data. saves the sorted list on LeaderboardData and returns it.
    /// </summary>
    /// <param name="scoreData"> the score data to append </param>
    /// <returns></returns>
    public static List<LeaderboardScoreData> AppendLeaderboardData(LeaderboardScoreData newScoreData, PlayerPath playerPath)
    {
        if (_scores == null)
        {
            _scores = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
        }

        if (!_scores.TryGetValue(playerPath, out List<LeaderboardScoreData> scoreData))
        {
            scoreData = new List<LeaderboardScoreData>();
            _scores.Add(playerPath, scoreData);
        }

        scoreData.Add(newScoreData);

        scoreData = UtilityFunctions.SortScoreDataByLowestScore(scoreData);
        scoreData = UtilityFunctions.ClampListLength(scoreData, 10);

        return scoreData;
    }
    //appends a single leaderboard data to the current scores. sorts and clams the list. sets it and returns it. takes a score data


    /// <summary>
    /// Returns the saved leaderboard data from LeaderboardData. make sure to save to it first if there is a change using LeaderboardData.CreateLeaderboardData() or LeaderboardData.AppendLeaderboardData()
    /// </summary>
    /// <returns></returns>
    public static List<LeaderboardScoreData> GetSavedScoreData(PlayerPath playerPath)
    {
        if (_scores == null)
        {
            _scores = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
        }
        if (!_scores.TryGetValue(playerPath, out List<LeaderboardScoreData> scoreData))
        {
            scoreData = new List<LeaderboardScoreData>();
            _scores.Add(playerPath, scoreData);
        }

        return scoreData;
    }


    /// <summary>
    /// Checks if a score will place on the on the leaderboard off of the saved scores. returns true if it will place, false if it won't
    /// </summary>
    /// <param name="timeScore"> the score to check </param>
    /// <returns></returns>
    public static bool CheckIfScorePlaces(float timeScore, PlayerPath playerPath)
    {
        if (_scores == null)
        {
            _scores = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
        }
        if (!_scores.TryGetValue(playerPath, out List<LeaderboardScoreData> scoreData))
        {
            scoreData = new List<LeaderboardScoreData>();
            _scores.Add(playerPath, scoreData);
        }

        if (_scores.Count >= 10)//there are 10 or more times on the board, so its competitive
        {
            if (scoreData[_scores.Count - 1].time < timeScore) //time is slower than 10th place (or the last place avaiable) dosen't place
            {
                return false;
            }
            else // time is equal to or quicker, time places
            {
                return true;
            }
        }
        else //less than 10 times on the leaderboard, time places by default
        {
            return true;
        }  
    }
    #endregion

    #region "Saving/Loading Functions"
    public static void SaveLeaderboard()
    {
        SaveManager.SaveData();
    }

    public static void LoadLeaderboard()
    {
        SaveManager.LoadSavedData();
        UtilityFunctions.CloneScoresDictionary(SaveManager.ScoreData, out _scores);
    }

    #endregion


}
#region "Data Structs"
[System.Serializable]
public struct LeaderboardScoreData
{
    public float time;
    public string name;
}
#endregion