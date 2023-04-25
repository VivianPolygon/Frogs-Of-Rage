using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatsTester : MonoBehaviour
{
    [SerializeField] PlayerHatManager trackedManager;

    private int _currentID;
    private string _idText;

    private void OnGUI()
    {
        _idText = GUILayout.TextArea(_idText);
        GUILayout.BeginHorizontal();
        _currentID = ConvertStringToValue(_idText);
        GUILayout.Label("ID: " + _currentID.ToString());
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Give Player Hat of ID"))
        {
            trackedManager.UnlockHat(_currentID);
        }
        if (GUILayout.Button("Equipt Hat of ID"))
        {
            trackedManager.EquiptHat(_currentID);
        }
        if (GUILayout.Button("Clear Player Inventory"))
        {
            trackedManager.EmptyPlayerInventory();
        }


        DisplayInventoryState();
    }

    private int ConvertStringToValue(string input)
    {
        if(input != null)
        {
            string modifiedstring = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsDigit(input[i]))
                {
                    modifiedstring += input[i];
                }
            }

            if (int.TryParse(modifiedstring, out int returnInt))
            {
                return returnInt;
            }
        }

        return 0;
    }

    private void DisplayInventoryState()
    {
        if(trackedManager != null)
        {
            if(trackedManager.Inventory != null)
            {
                HatDatabaseRetreiver retreiver = new HatDatabaseRetreiver();
                List<HatData> databaseList = trackedManager.Inventory._databaseCopy;
                GUILayout.BeginVertical();
                foreach (HatData data in databaseList)
                {
                    if(trackedManager.Inventory._playerHats.TryGetValue(data.hatID, out bool state))
                    {
                        GUILayout.Label("ID: " + data.hatID + " Name: " + data.hatName + " Player Has: " + state.ToString());
                    }
                }
                GUILayout.EndVertical();
            }
        }
    }
}
