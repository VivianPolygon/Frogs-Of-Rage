using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Events;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveManager
{
    #region "Leaderboard Saving Variables"
    private static readonly string _leaderboardFilePath = Application.persistentDataPath + "/Frogs.rage";

    private static LeaderboardSaveData _loadedScoresData;
    public static LeaderboardSaveData LoadedScoresData
    {
        get
        {
            if (_loadedScoresData == null)
            {
                _loadedScoresData = new LeaderboardSaveData(new Dictionary<int, float[]>(), new Dictionary<int, string[]>());
            }
            return _loadedScoresData;
        }
    } //public static property for reading the loaded data

    private static Dictionary<PlayerPath, List<LeaderboardScoreData>> _scoreData;
    public static Dictionary<PlayerPath, List<LeaderboardScoreData>> ScoreData
    {
        get
        {
            return _scoreData;
        }
        set
        {
            _scoreData = value;
        }
    }

    private static DevTimesLoader devTimesLoader;
    public static DevScoresLoaded devScoresLoaded;
    #endregion

    #region "Hat Inventory Saving Variables"
    private static readonly string _hatInventoryFilePath = Application.persistentDataPath + "/Hats.rage";

    private static Dictionary<int, bool> _loadedHatsData;
    public static Dictionary<int, bool> LoadedHatsData
    {
        get
        {
            if (_loadedHatsData == null)
            {
                _loadedHatsData = new Dictionary<int, bool>();
            }
            return _loadedHatsData;
        }
    } //public static property for reading the loaded data. returns the list format.
    #endregion

    #region "Leaderboard Saving Funtions"
    public static void SaveLeaderboardData()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream fileStream = new FileStream(_leaderboardFilePath, FileMode.Create);

        LeaderboardSaveData saveData = LeaderboardConvertToSaveFormat();

        formatter.Serialize(fileStream, LeaderboardConvertToSaveFormat());

        fileStream.Close();
    }

    /// <summary>
    /// Sets the static public getOnly LoadedData Property to the load data generated. 
    /// </summary>
    public static void LoadLeaderboardSavedData()
    {
        LeaderboardSaveData loadData = null;
        if (File.Exists(_leaderboardFilePath))
        {
            Debug.Log("File found");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(_leaderboardFilePath, FileMode.Open);

            if(fileStream.Length > 0)
            {
                //loads the data if the filestream has contents
                loadData = formatter.Deserialize(fileStream) as LeaderboardSaveData;

            }

            fileStream.Flush();
            fileStream.Close();

        }
        else
        {
            File.Create(_leaderboardFilePath); //creates file
        }

        if(loadData == null)
        {
            Debug.Log("NO Data found");
            loadData = new LeaderboardSaveData(new Dictionary<int, float[]>(), new Dictionary<int, string[]>());
            GetDefaultScoresFromJson();
        }

        _loadedScoresData = loadData;
        _scoreData = _loadedScoresData.ConvertSaveToLeaderboardData();
    }

    public static void EraseLeaderboardSaveData() //erases save data 
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(_leaderboardFilePath, FileMode.Create);

        binaryFormatter.Serialize(fileStream, _loadedScoresData = new LeaderboardSaveData(new Dictionary<int, float[]>(), new Dictionary<int, string[]>()));

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


    public static void GetDefaultScoresFromJson()
    {
        Dictionary<PlayerPath, List<LeaderboardScoreData>> defaultScores = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();

        if(devTimesLoader == null)
        {
            devTimesLoader = new DevTimesLoader();
        }
        if (devScoresLoaded == null)
        {
            devScoresLoaded = new DevScoresLoaded();
        }

        devTimesLoader.LoadDevTimes(); //loads the dev times. takes time due to being asyncronous.
        devTimesLoader.loadEvent.AddListener(ProcessDevTimes);
        Debug.Log("Getting Times");
        
    }

    private static void ProcessDevTimes(TextAsset jsonAsset)
    {
        if (jsonAsset != null)
        {
            string readJsonString = jsonAsset.text;

            if (readJsonString.Length > 0)
            {
                DefaultScoresJsonFormatted loadedDefaultScores = JsonUtility.FromJson<DefaultScoresJsonFormatted>(readJsonString);

                if (loadedDefaultScores != null)
                {
                    Debug.Log("Default Scores Loaded Sucsessfully");
                    loadedDefaultScores.LoadDictionary(out _scoreData);
                }
                else
                {
                    Debug.LogError("Failed To Load Scores");
                    _scoreData = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
                }
            }
            else
            {
                Debug.Log("Json was Empty");
                _scoreData = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
            }

        }
        else
        {
            Debug.Log("Failed to retreive json from asset bundle");
            _scoreData = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
        }

        //number dosent matter. it just needs an argument to work, can't work argumentless.
        devScoresLoaded.Invoke(0);
    }

    //used to chekc if scores exsist, for adding in default scores
    public static bool CheckForScoreSaves(Dictionary<PlayerPath, List<LeaderboardScoreData>> dictionaryToCheck)
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

    #endregion

    #region "Hat Saving Functions"
    public static void SavePlayerHats(Dictionary<int, bool> inventory)
    {
        if(inventory != null)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream fileStream = new FileStream(_hatInventoryFilePath, FileMode.Create);

            formatter.Serialize(fileStream, new HatInventorySaveData(inventory));

            fileStream.Flush();
            fileStream.Close();
        }
    }

    public static Dictionary<int, bool> LoadPlayerHats()
    {
        if (File.Exists(_hatInventoryFilePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(_hatInventoryFilePath, FileMode.Open);

            HatInventorySaveData hatLoadData = formatter.Deserialize(fileStream) as HatInventorySaveData;

            fileStream.Flush();
            fileStream.Close();

            _loadedHatsData = hatLoadData.savedHatInventory;


        }
        else
        {
            _loadedHatsData = new Dictionary<int, bool>();
        }
        return _loadedHatsData;
    }

    public static void EraseHatInventorySave()
    {
        if (File.Exists(_hatInventoryFilePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(_hatInventoryFilePath, FileMode.Create);

            binaryFormatter.Serialize(fileStream, new HatInventorySaveData(new Dictionary<int, bool>()));

            fileStream.Close();
        }
    }


    #endregion
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

[System.Serializable]
public class HatInventorySaveData
{
    public Dictionary<int, bool> savedHatInventory;

    public HatInventorySaveData(Dictionary<int, bool> dataToSave)
    {
        if(dataToSave != null)
        {
            savedHatInventory = dataToSave;
        }
        else
        {
            savedHatInventory = new Dictionary<int, bool>();
        }
    }

}

public class DevScoresLoaded : UnityEvent<int> { }

