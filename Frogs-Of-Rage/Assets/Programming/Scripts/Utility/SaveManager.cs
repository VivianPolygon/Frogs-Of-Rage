using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    private static string _filePath = Application.persistentDataPath + "/Frogs.rage";

    private static LeaderboardSaveData _loadedData;
    public static LeaderboardSaveData LoadedData
    {
        get
        {
            if (_loadedData == null)
            {
                _loadedData = new LeaderboardSaveData(new Dictionary<int, float[]>(), new Dictionary<int, string[]>());
            }
            return _loadedData;
        }
    } //public static property for reading the loaded data

    private static Dictionary<PlayerPath, List<LeaderboardScoreData>> _scoreData;
    public static Dictionary<PlayerPath, List<LeaderboardScoreData>> ScoreData
    {
        get
        {
            if(!CheckForScoreSaves(_scoreData))
            {
                _scoreData = GetDefaultScoresFromJson();
            }
            return _scoreData;
        }
        set
        {
            _scoreData = value;
        }
    }

    public static void SaveData()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream fileStream = new FileStream(_filePath, FileMode.Create);

        LeaderboardSaveData saveData = LeaderboardConvertToSaveFormat();

        formatter.Serialize(fileStream, LeaderboardConvertToSaveFormat());

        fileStream.Close();
    }

    /// <summary>
    /// Sets the static public getOnly LoadedData Property to the load data generated. 
    /// </summary>
    public static void LoadSavedData()
    {
        if(File.Exists(_filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(_filePath, FileMode.Open);

            LeaderboardSaveData loadData = formatter.Deserialize(fileStream) as LeaderboardSaveData;
            fileStream.Close();

            _loadedData = loadData;
            _scoreData = loadData.ConvertSaveToLeaderboardData();

        }
        else
        {
            _loadedData = new LeaderboardSaveData(new Dictionary<int, float[]>(), new Dictionary<int, string[]>());
            _scoreData = _loadedData.ConvertSaveToLeaderboardData();

        }
    }

    public static void EraseSaveData() //erases save data 
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(_filePath, FileMode.Create);

        binaryFormatter.Serialize(fileStream, _loadedData = new LeaderboardSaveData(new Dictionary<int, float[]>(), new Dictionary<int, string[]>()));

        fileStream.Close();
    }
    private static LeaderboardSaveData LeaderboardConvertToSaveFormat()
    {
        List<int> usedPaths = new List<int>();
        Dictionary<int, float[]> scores = new Dictionary<int, float[]>();
        Dictionary<int, string[]> times = new Dictionary<int, string[]>();

        foreach (int playerPath in Enum.GetValues(typeof(PlayerPath)))
        {
            List<LeaderboardScoreData> currentData = LeaderboardData.GetSavedScoreData((PlayerPath)playerPath);
            if (currentData.Count > 0) //makes sure there is content to save
            {
                float[] newScores = new float[currentData.Count];
                string[] newNames = new string[currentData.Count];
                
                for (int i = 0; i < currentData.Count; i++)
                {
                    newScores[i] = currentData[i].time;
                    newNames[i] = currentData[i].name;
                }

                scores.Add(playerPath, newScores);
                times.Add(playerPath, newNames);
            }
        }

        LeaderboardSaveData saveResults = new LeaderboardSaveData(scores, times);
        return saveResults;
    }


    public static Dictionary<PlayerPath, List<LeaderboardScoreData>> GetDefaultScoresFromJson()
    {
        Dictionary<PlayerPath, List<LeaderboardScoreData>> defaultScores = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();

        if (!File.Exists(Application.dataPath + "/Resources/DefaultScores.json"))
        {
            File.Create(Application.dataPath + "/Resources/DefaultScores.json");
            Debug.LogWarning("Json file didn't exsist, it has been created, please try loading again");

            return defaultScores;
        }

        string readJsonString = "";

        FileStream readingStream = new FileStream(Application.dataPath + "/Resources/DefaultScores.json", FileMode.Open, FileAccess.Read);

        if (readingStream.CanRead)
        {
            byte[] buffer = new byte[readingStream.Length];
            int bytesRead = readingStream.Read(buffer, 0, buffer.Length);

            readJsonString = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
        else
        {
            Debug.LogError("Couldnt read from file. Default Leaderboards not Loaded");
        }

        readingStream.Flush();
        readingStream.Close();

        if (readJsonString.Length > 0)
        {
            DefaultScoresJsonFormatted loadedDefaultScores = JsonUtility.FromJson<DefaultScoresJsonFormatted>(readJsonString);

            if (loadedDefaultScores != null)
            {
                Debug.Log("Default Scores Loaded Sucsessfully");
                loadedDefaultScores.LoadDictionary(out defaultScores);
            }
            else
            {
                Debug.LogError("Failed To Load Scores");
                defaultScores = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
            }
        }
        return defaultScores;
    }

    //used to chekc if scores exsist, for adding in default scores
    private static bool CheckForScoreSaves(Dictionary<PlayerPath, List<LeaderboardScoreData>> dictionaryToCheck)
    {
        //empty check, fills with defaults if empty
        int scoreQuantity = 0;
        List<LeaderboardScoreData> currentList;

        foreach (int value in Enum.GetValues(typeof(PlayerPath)))
        {

            if (dictionaryToCheck.TryGetValue((PlayerPath)value, out currentList))
            {
                scoreQuantity += currentList.Count;
            }
        }

        if (scoreQuantity < 1)
        {
            return false;
        }

        return true;
    }

}
[System.Serializable]
public class LeaderboardSaveData
{
    private readonly int[] paths;
    private readonly Dictionary<int, float[]> scores;
    private readonly Dictionary<int, string[]> names;

    public LeaderboardSaveData(Dictionary<int, float[]> newScores, Dictionary<int, string[]> newtimes)//conttructor 
    {
        scores = newScores;
        names = newtimes;


    }
   
    public Dictionary<PlayerPath, List<LeaderboardScoreData>> ConvertSaveToLeaderboardData()
    {
        Dictionary<PlayerPath, List<LeaderboardScoreData>> savedLeaderboardData = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();

        foreach (int playerPath in Enum.GetValues(typeof(PlayerPath)))
        {
            float[] pathScores; scores.TryGetValue(playerPath, out pathScores);
            string[] pathNames; names.TryGetValue(playerPath, out pathNames);

            List<LeaderboardScoreData> pathScoreData = new List<LeaderboardScoreData>();

            if(pathScores != null && pathNames != null)
            {
                for (int i = 0; i < pathScores.Length; i++)
                {
                    //Debug.Log(UtilityFunctions.FormatStringFirstLetterCapitalized(Enum.GetName(typeof(PlayerPath), (PlayerPath)playerPath)) + " Leaderboard Place " + (i + 1).ToString() + " : Score: " + pathScores[i].ToString() + " Name: " + pathNames[i]);
                    LeaderboardScoreData LoadedData;
                    LoadedData.time = pathScores[i];
                    LoadedData.name = pathNames[i];

                    pathScoreData.Add(LoadedData);
                }
            }

            savedLeaderboardData.Add((PlayerPath)playerPath, pathScoreData);
        }

        return savedLeaderboardData;
    }


}


