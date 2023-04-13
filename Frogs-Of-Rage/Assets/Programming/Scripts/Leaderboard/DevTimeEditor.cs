using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;

#if UNITY_EDITOR 
using UnityEditor;

public class DevTimeEditor : EditorWindow
{
    private static string filePath = "/Resources/DefaultScores.json";

    private static EditorWindow _editorInstance;
    private Dictionary<PlayerPath, List<LeaderboardScoreData>> _defaultScores;

    private List<LeaderboardScoreData> _currentDisplayList;
    private PlayerPath _currentDisplayPlayerPath;

    //listcolors, for ease of differentiation
    Color listColor1 = new Color(0.2f, 0.2f, 0.2f);
    Color listColor2 = new Color(0.35f, 0.35f, 0.35f);

    [MenuItem("Frog Editors/Edit Dev Times", priority = 0)]
    public static void ShowDefaultScoreEditor() //creates editor window when tab is clicked
    {
        _editorInstance = GetWindow<DevTimeEditor>("Edit Default Scoreboards");
        _editorInstance.minSize = new Vector2Int(500, 500);
        _editorInstance.maxSize = new Vector2Int(500, 500);
    }

    private void Awake()
    {
        //initial load
        LoadFromJson(Application.dataPath + filePath);

        //initialy sets display to first option in enum
        string[] playerPaths = Enum.GetNames(typeof(PlayerPath));
        if(playerPaths != null && playerPaths.Length > 0)
        {
            DisplayList(playerPaths[0]);
        }

    }

    private void OnGUI()
    {

        TopBarDisplay();
        AddingRemovingButtons();
        CurrentListDisplay();
        ListDisplay();
        SaveSortList();
    }


    #region"Dislay Main Functions"
    private void TopBarDisplay()
    {
        EditorGUILayout.BeginHorizontal();

        foreach (string enumName in Enum.GetNames(typeof(PlayerPath)))
        {
            if(GUILayout.Button(enumName))
            {
                DisplayList(enumName);
            }
        }

        EditorGUILayout.EndHorizontal();
    } //horizontal strip

    private void AddingRemovingButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Add Default Score"))
        {
            AddScore();
        }
        if (GUILayout.Button("Remove Default Score"))
        {
            RemoveScore();
        }

        EditorGUILayout.EndHorizontal();
    } //horizontal strip

    private void CurrentListDisplay()
    {
        EditorGUILayout.BeginHorizontal(); 
        EditorGUILayout.LabelField("Current board: " + UtilityFunctions.FormatStringFirstLetterCapitalized(Enum.GetName(typeof(PlayerPath), _currentDisplayPlayerPath)));
        EditorGUILayout.EndHorizontal();
    } //horizontal strip

    private void ListDisplay() // vertical, displays variable element amounts, between 0 and 10 
    {
        if(_currentDisplayList != null)
        {
            for (int i = 0; i < _currentDisplayList.Count; i++)
            {
                _currentDisplayList[i] = DisplayEditableElement(_currentDisplayList[i], i);
            }
        }
    }

    private void SaveSortList() //horizontal strip
    {
        GUILayout.BeginArea(new Rect(0, 485, 500, 50));
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Sort Scores") && _currentDisplayList != null)
        {
            SortScores(_currentDisplayList);
        }
        if (GUILayout.Button("Sort + Save Scores") && _currentDisplayList != null)
        {
            SortScores(_currentDisplayList);
            SaveToJson(Application.dataPath + filePath);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    #endregion

    #region"Display Helper Functions"
    //ListDisplay
    private void DisplayList(string enumName)
    {
        if (_defaultScores == null) //checks and gets scores if it needs to  
        {
            GetDefaultScores();
        }

        LocalSaveList();

        PlayerPath currentPath = (PlayerPath)Enum.Parse(typeof(PlayerPath), enumName);

        _defaultScores.TryGetValue(currentPath, out _currentDisplayList);
        if (_currentDisplayList == null)
        {
            _currentDisplayList = new List<LeaderboardScoreData>();
        }
        _currentDisplayPlayerPath = currentPath;
    }

    //individual display score function.
    private LeaderboardScoreData DisplayEditableElement(LeaderboardScoreData scoreData, int scoreIndex) //displays each score data in an organized horizontal chunck
    {
        //sets alternating color
        Color displayColor;
        if(scoreIndex % 2 == 1)
        {
            displayColor = listColor1;
        }
        else
        {
            displayColor = listColor2;
        }

        float offset = 75 + (scoreIndex * 40);

        EditorGUI.DrawRect(new Rect(0, offset, 500, 40), displayColor);
        GUILayout.BeginArea(new Rect(0, offset, 500, 40));
        GUILayout.BeginHorizontal();

        GUILayout.Label((scoreIndex + 1).ToString());
        GUILayout.Label("Name: ");
        scoreData.name = RestrictNameText(GUILayout.TextField(scoreData.name));

        GUILayout.Label("Time: ");
        scoreData.time = float.Parse(RestrictTimeText(GUILayout.TextField(scoreData.time.ToString())));
        GUILayout.Label(UtilityFunctions.ConvertSecondsToStandardTimeFormatString(scoreData.time));

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
        return scoreData;

    }

    //sort function
    private List<LeaderboardScoreData> SortScores (List<LeaderboardScoreData> scoreDataList)
    {
        scoreDataList = UtilityFunctions.SortScoreDataByLowestScore(scoreDataList);
        return scoreDataList;
    }


    //string entering restrictors
    private string RestrictNameText(string restrictedNameText)
    {
        if(restrictedNameText != null)
        {
            string newString = "";
            for (int i = 0; i < restrictedNameText.Length; i++)
            {
                if (char.IsLetter(restrictedNameText[i]))
                {
                    newString += char.ToUpper(restrictedNameText[i]);
                    if(newString.Length == 3)
                    {
                        return newString;
                    }
                }
            }
            return newString;
        }
        return "";
    }
    private string RestrictTimeText(string restrictedTimeText)
    {
        if(restrictedTimeText != null)
        {
            string newString = "";
            bool periodAdded = false;

            for (int i = 0; i < restrictedTimeText.Length; i++)
            {
                if (char.IsDigit(restrictedTimeText[i]))
                {
                    newString += restrictedTimeText[i];
                }
                else if (restrictedTimeText[i] == char.Parse(".") && !periodAdded)
                {
                    newString += restrictedTimeText[i];
                    periodAdded = true;
                }
            }

            if(newString.Length > 0)
            {
                return newString;
            }
            else
            {
                return "60";
            }
        }
        return "0";
    }

    //Add/Remove Button Functions
    private void AddScore()
    {
        if(_currentDisplayList != null)
        {
            if(_currentDisplayList.Count < 10)
            {
                _currentDisplayList.Add(new LeaderboardScoreData());
            }
            else
            {
                Debug.Log("Default leaderboard is already full");
            }
        }
    }
    private void RemoveScore()
    {
        if (_currentDisplayList != null)
        {
            if (_currentDisplayList.Count > 0)
            {
                _currentDisplayList.Remove(_currentDisplayList[_currentDisplayList.Count - 1]);
            }
            else
            {
                Debug.Log("Default leaderboard is already Empty");
            }
        }
    }

    //temporary save to the editor window
    private void LocalSaveList()
    {
        //saves current list to dictionary before switch
        if (_defaultScores != null && _currentDisplayList != null)
        {
            if(_defaultScores.TryGetValue(_currentDisplayPlayerPath, out List<LeaderboardScoreData> datalist))
            {
                if (datalist != _currentDisplayList)
                {
                    _defaultScores.Remove(_currentDisplayPlayerPath);
                    _defaultScores.Add(_currentDisplayPlayerPath, _currentDisplayList); 
                }
            }
            else
            {
                _defaultScores.Add(_currentDisplayPlayerPath, _currentDisplayList);
            }
        }
    }

    #endregion

    #region"Json Serilization"
    //dictionary getting
    public void GetDefaultScores()
    {
        LoadFromJson(Application.dataPath + filePath);

        if(_defaultScores == null)
        {
            _defaultScores = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
        }
    }

    private void SaveToJson(string filepath)
    {
        if(!File.Exists(filepath))
        {
            File.Create(filepath);
            Debug.LogWarning("Json file didn't exsist, it has been created, please try saving again");
            return;
        }


        if(_defaultScores != null)
        {                
            string jsonString = JsonUtility.ToJson(new DefaultScoresJsonFormatted(_defaultScores));

            FileStream writingStream = new FileStream(filepath, FileMode.Create, FileAccess.Write);

            if(writingStream.CanWrite)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(jsonString);

                writingStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                Debug.LogError("Couldnt write to file. Leaderboards not Saved");
            }
            
            writingStream.Flush();
            writingStream.Close();
        }
        else
        {
            Debug.LogWarning("Nothing to save");
        }
    }

    //used internaly
    private void LoadFromJson(string filepath)
    {
        if (!File.Exists(filepath))
        {
            File.Create(filepath);
            Debug.LogWarning("Json file didn't exsist, it has been created, please try loading again");
            return;
        }

        string readJsonString = "";

        FileStream readingStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);

        if (readingStream.CanRead)
        {
            byte[] buffer = new byte[readingStream.Length];
            int bytesRead = readingStream.Read(buffer, 0, buffer.Length);

            readJsonString = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
        else
        {
            Debug.LogError("Couldnt read from file. Leaderboards not Loaded");
        }

        readingStream.Flush();
        readingStream.Close();

        if(readJsonString.Length > 0)
        {
            DefaultScoresJsonFormatted loadedDefaultScores = JsonUtility.FromJson<DefaultScoresJsonFormatted>(readJsonString);

            if(loadedDefaultScores != null)
            {
                Debug.Log("Default Scores Loaded Sucsessfully");
                loadedDefaultScores.LoadDictionary(out _defaultScores);
            }
            else
            {
                Debug.LogError("Failed To Load Scores");
            }
        }
    }

    #endregion


}

#endif

[System.Serializable]
public class DefaultScoresJsonFormatted
{
    public List<LeaderboardScoreData> scoreData;

    public List<string> pathEnumString;
    public List<int> pathEnumIndexing;

    public DefaultScoresJsonFormatted(Dictionary<PlayerPath, List<LeaderboardScoreData>> inputDictionary)
    {
        //initilizes data
        scoreData = new List<LeaderboardScoreData>();
        pathEnumString = new List<string>();
        pathEnumIndexing = new List<int>();


        List<LeaderboardScoreData> currentList;

        foreach(int value in Enum.GetValues(typeof(PlayerPath)))
        {
            if(inputDictionary.TryGetValue((PlayerPath)value, out currentList))
            {
                for (int i = 0; i < currentList.Count; i++)
                {
                    scoreData.Add(currentList[i]);
                }

                pathEnumString.Add(Enum.GetName(typeof(PlayerPath), (PlayerPath)value));
                pathEnumIndexing.Add(currentList.Count);
            }
        }
    }

    public void LoadDictionary(out Dictionary<PlayerPath, List<LeaderboardScoreData>> loadedDictionary)
    {
        loadedDictionary = new Dictionary<PlayerPath, List<LeaderboardScoreData>>();
        int mainIndexer = 0;
        int subIndexer = 0;

        for (int i = 0; i < pathEnumString.Count; i++)
        {
            List<LeaderboardScoreData> currentList = new List<LeaderboardScoreData>();

            while(subIndexer < pathEnumIndexing[i])
            {
                currentList.Add(scoreData[subIndexer + mainIndexer]);

                subIndexer++;
            }


            loadedDictionary.Add((PlayerPath)Enum.Parse(typeof(PlayerPath), pathEnumString[i]), currentList);

            mainIndexer += pathEnumIndexing[i];
            subIndexer = 0;
        }
        //DebugPrintValues(loadedDictionary);
    }

    public void DebugPrintValues(Dictionary<PlayerPath, List<LeaderboardScoreData>> debugLoadedDictionary)
    {

        if(debugLoadedDictionary == null)
        {
            Debug.Log("DictionaryDebug: Dictionary was null");
        }
        else
        {
            Debug.Log("DictionaryDebug: Dictionary has <b>" + debugLoadedDictionary.Count + "</b> keys");


            foreach(int key in Enum.GetValues(typeof(PlayerPath)))
            {
                string values = "";
                List<LeaderboardScoreData> currentData = new List<LeaderboardScoreData>();
                if(debugLoadedDictionary.TryGetValue((PlayerPath)key, out currentData))
                {
                    foreach(LeaderboardScoreData data in currentData)
                    {
                        values += data.name + ", ";
                    }
                }

                Debug.Log("Dictionary Debug: Key <b>" + Enum.GetName(typeof(PlayerPath), (PlayerPath)key) + "</b> has values: <b> " + values + "</b>");
            }
        }

    }
}
