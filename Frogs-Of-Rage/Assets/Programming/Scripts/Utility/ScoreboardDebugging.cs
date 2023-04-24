using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreboardDebugging : MonoBehaviour
{
    public PlayerController _playerController;

    private void OnGUI()
    {

        if(_playerController)
        {
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Player Win From Vent"))
            {
                PlayerController.OnPlayerWin(new PlayerWinEventArgs(GameManager.Instance.gameTimer, _playerController.playerData, PlayerPath.VENT));
            }
            if (GUILayout.Button("Player Win From Outlet"))
            {
                PlayerController.OnPlayerWin(new PlayerWinEventArgs(GameManager.Instance.gameTimer, _playerController.playerData, PlayerPath.OUTLET));
            }
            if (GUILayout.Button("Player Win From Wall"))
            {
                PlayerController.OnPlayerWin(new PlayerWinEventArgs(GameManager.Instance.gameTimer, _playerController.playerData, PlayerPath.WALL));
            }
            GUILayout.EndHorizontal();
        }

        if(GameManager.Instance)
        {
            if(GameManager.Instance.gameTimer)
            {
                if(GUILayout.Button("ResetGameTimer"))
                {
                    GameManager.Instance.gameTimer.totalTime = 0;
                }

                GUI.color = Color.black;
                GUILayout.Label("Current Game Timer Time: <b>" +  UtilityFunctions.ConvertSecondsToStandardTimeFormatString(GameManager.Instance.gameTimer.totalTime) + "</b>");
            }
        }

        if(GUILayout.Button("EnumTest"))
        {
            print(UtilityFunctions.FormatStringFirstLetterCapitalized(Enum.GetName(typeof(PlayerPath), PlayerPath.OUTLET)));
        }

        GUILayout.BeginHorizontal();
        GUI.color = Color.red;
        if (GUILayout.Button("Erase Save Data"))
        {
            SaveManager.EraseLeaderboardSaveData();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUI.color = Color.blue;
        if (GUILayout.Button("Dev Scores Test"))
        {
            Debug.Log("Button dosent work anymore, sorry");
            //Dictionary<PlayerPath, List<LeaderboardScoreData>> testDict;

            //UtilityFunctions.CloneScoresDictionary(SaveManager.GetDefaultScoresFromJson(), out testDict);


        }
        GUILayout.EndHorizontal();

    }




    #region"InspectorDisplayScores"
    //exposes scores in the inspector
    private List<LeaderboardScoreData> ventData;
    private List<LeaderboardScoreData> wallData;
    private List<LeaderboardScoreData> outletData;

    [Header("Vent")]
    [Space(25)]
    [SerializeField] List<float> ventTimes;
    [SerializeField] List<string> ventNames;

    [Header("Wall")]
    [Space(25)]
    [SerializeField] List<float> wallTimes;
    [SerializeField] List<string> wallNames;

    [Header("Outlet")]
    [Space(25)]
    [SerializeField] List<float> outletTimes;
    [SerializeField] List<string> outletNames;

    private void Update()
    {
        ventData = LeaderboardData.GetSavedScoreData(PlayerPath.VENT);
        wallData = LeaderboardData.GetSavedScoreData(PlayerPath.WALL);
        outletData = LeaderboardData.GetSavedScoreData(PlayerPath.OUTLET);

        ventTimes = new List<float>();
        ventNames = new List<string>();
        for (int i = 0; i < ventData.Count; i++)
        {
            ventTimes.Add(ventData[i].time);
            ventNames.Add(ventData[i].name);
        }

        wallTimes = new List<float>();
        wallNames = new List<string>();
        for (int i = 0; i < wallData.Count; i++)
        {
            wallTimes.Add(wallData[i].time);
            wallNames.Add(wallData[i].name);
        }

        outletTimes = new List<float>();
        outletNames = new List<string>();
        for (int i = 0; i < outletData.Count; i++)
        {
            outletTimes.Add(outletData[i].time);
            outletNames.Add(outletData[i].name);
        }
    }
    #endregion
}
