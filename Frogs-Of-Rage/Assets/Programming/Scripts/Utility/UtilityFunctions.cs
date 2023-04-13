using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UtilityFunctions
{
    /// <summary>
    /// Sorts list of floats, lowest number is first.
    /// </summary>
    /// <param name="listToSort"></param>
    /// <returns></returns>
    public static List<float> SortFloatListLowestFirst(List<float> listToSort)
    {
        if(listToSort != null)
        {
            listToSort.Sort(SortFunctions.SortByLowestValue);
            return listToSort;
        }
        else
        {
            return null;
        }
    }

    public static List<LeaderboardScoreData> SortScoreDataByLowestScore(List<LeaderboardScoreData> scoreDataListToSort)
    {
        if (scoreDataListToSort != null)
        {
            scoreDataListToSort.Sort(SortFunctions.SortByLowestScoreDataScore);
            return scoreDataListToSort;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a float inputed for seconds to a more standard format. hours:minutes:seconds:milliseconds. if a field is 0, culls it, so 5 minutes would be 5:00:00.
    /// </summary>
    /// <param name="timeInSeconds"> time in seconds </param>
    /// <returns></returns>
    public static string ConvertSecondsToStandardTimeFormatString(float timeInSeconds)
    {
        string timeString;
        if(timeInSeconds < 60) //less than a minute, only needs seconds and milliseconds
        {
            int seconds = Mathf.FloorToInt(timeInSeconds);
            int milliseconds = Mathf.FloorToInt((timeInSeconds - seconds) * 1000);

            string secondsString;
            if (seconds < 10) secondsString = "0" + seconds.ToString() + ":";
            else { secondsString = seconds.ToString() + ":"; }

            string millisecondsString;
            if (milliseconds < 10) millisecondsString = "00" + milliseconds.ToString();
            else if (milliseconds < 100) millisecondsString = "0" + milliseconds.ToString();
            else millisecondsString = milliseconds.ToString();

            timeString = (secondsString + millisecondsString);
        }
        else if(timeInSeconds < 60 * 60) //less than an hour, only needs minutes, seconds and milliseconds
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60);
            int seconds = Mathf.FloorToInt(timeInSeconds - (minutes * 60));
            int milliseconds = Mathf.FloorToInt((timeInSeconds - (minutes * 60) - seconds) * 1000);

            string minutesString;
            if (minutes < 10) minutesString = "0" + minutes.ToString() + ":";
            else { minutesString = minutes.ToString() + ":"; }

            string secondsString;
            if (seconds < 10) secondsString = "0" + seconds.ToString() + ":";
            else { secondsString = seconds.ToString() + ":"; }

            string millisecondsString;
            if (milliseconds < 10) millisecondsString = "00" + milliseconds.ToString();
            else if (milliseconds < 100) millisecondsString = "0" + milliseconds.ToString();
            else millisecondsString = milliseconds.ToString();

            timeString = (minutesString + secondsString + millisecondsString);
        }
        else //time is over an hour
        {
            int hours = Mathf.FloorToInt(timeInSeconds / (60 * 60));
            int minutes = Mathf.FloorToInt(timeInSeconds / 60 - (hours * (60 * 60)));
            int seconds = Mathf.FloorToInt(timeInSeconds - (hours * (60 * 60) - (minutes * 60)));
            int milliseconds = Mathf.FloorToInt((timeInSeconds - (hours * (60 * 60) - (minutes * 60) - seconds) * 1000));

            string hoursString;
            if (hours < 10) hoursString = "0" + hours.ToString() + ":";
            else { hoursString = hours.ToString() + ":"; }

            string minutesString;
            if (minutes < 10) minutesString = "0" + minutes.ToString() + ":";
            else { minutesString = minutes.ToString() + ":"; }

            string secondsString;
            if (seconds < 10) secondsString = "0" + seconds.ToString() + ":";
            else { secondsString = seconds.ToString() + ":"; }

            string millisecondsString;
            if (milliseconds < 10) millisecondsString = "00" + milliseconds.ToString();
            else if (milliseconds < 100) millisecondsString = "0" + milliseconds.ToString();
            else millisecondsString = milliseconds.ToString();

            timeString = (hoursString + minutesString + secondsString + millisecondsString);
        }

        return timeString;
    }

    /// <summary>
    /// Clamps a list to a given klength, culling extra elements
    /// </summary>
    /// <typeparam name="T">the type of the list</typeparam>
    /// <param name="list">the list to clamp</param>
    /// <param name="maxCount">the maximum length of the list returned</param>
    /// <returns></returns>
    public static List<T> ClampListLength<T>(List<T> list, int maxCount)
    {
        if (list != null)
        {
            if (list.Count > maxCount)
            {
                List<T> newList = new List<T>();
                for (int i = 0; i < maxCount; i++)
                {
                    newList.Add(list[i]);
                }

                return newList;
            }

            return list;
        }

        Debug.LogWarning("attempted to clamp a null list on Leaderboard data in ClampListLength");
        return null;
    }

    public static string FormatStringFirstLetterCapitalized(string inputString)
    {
        char[] characters = new char[inputString.Length];
        string returnString = "";

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i] = inputString[i];

            if(char.IsLetter(characters[i]))
            {
                if(i == 0)
                {
                    characters[i] = char.ToUpper(characters[i]);
                }
                else
                {
                    characters[i] = char.ToLower(characters[i]);
                }
            }
            returnString += characters[i];
        }

        return returnString;
    }

    public static List<HatData> SortByHatID(List<HatData> input)
    {
        input.Sort(SortFunctions.SortByLowestHatID);
        return input;
    }




    public class SortFunctions : IComparer
    {
        public int Compare(object x, object y)
        {
            throw new NotImplementedException();
        }

        public static int SortByLowestScoreDataScore(LeaderboardScoreData a, LeaderboardScoreData b)
        {
            if (a.time > b.time)
            {
                return 1;
            }

            if (a.time < b.time)
            {
                return -1;
            }
            return 0;
        }

        public static int SortByLowestValue(float a, float b)
        {
            if (a > b)
            {
                return 1;
            }

            if (a < b)
            {
                return -1;
            }
            return 0;

        }

        public static int SortByLowestHatID(HatData a, HatData b)
        {
            if (a.hatID > b.hatID)
            {
                return 1;
            }

            if (a.hatID < b.hatID)
            {
                return -1;
            }
            return 0;
        }
    }

    //clones a new refrence type of a scores dictionary
    public static void CloneScoresDictionary(Dictionary<PlayerPath, List<LeaderboardScoreData>> dictionary, out Dictionary<PlayerPath, List<LeaderboardScoreData>> copy)
    {
        copy = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();

        foreach(int value in Enum.GetValues(typeof(PlayerPath)))
        {
            List<LeaderboardScoreData> currentList;
            List<LeaderboardScoreData> clonedList;
            if(dictionary.TryGetValue((PlayerPath)value, out currentList))
            {
                clonedList = new List<LeaderboardScoreData>();
                for (int i = 0; i < currentList.Count; i++)
                {
                    clonedList.Add(currentList[i]);
                }

                clonedList = SortScoreDataByLowestScore(clonedList);
                copy.Add((PlayerPath)value, clonedList);
            }
        }
    }
}
