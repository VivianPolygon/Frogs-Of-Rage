using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardDebugging : MonoBehaviour
{
    public PlayerController _playerController;


    //Commented out because PlayerWinEventArgs was throwing a compiler error
    //private void OnGUI()
    //{

    //    if(_playerController)
    //    {
    //        if(GUILayout.Button("PlayerWinTrigger"))
    //        {
    //            PlayerController.OnPlayerWin(new PlayerWinEventArgs(GameManager.Instance.gameTimer, _playerController.playerData));
    //        }
    //    }

    //    if(GameManager.Instance)
    //    {
    //        if(GameManager.Instance.gameTimer)
    //        {
    //            if(GUILayout.Button("ResetGameTimer"))
    //            {
    //                GameManager.Instance.gameTimer.totalTime = 0;
    //            }

    //            GUI.color = Color.black;
    //            GUILayout.Label("Current Game Timer Time: <b>" +  UtilityFunctions.ConvertSecondsToStandardTimeFormatString(GameManager.Instance.gameTimer.totalTime) + "</b>");
    //        }
    //    }

    //}
}
