using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HatPickup))]
public class HatPickupEditor : Editor
{
    //set on the script in the project folder
    [SerializeField] private HatDatabase _database;

    private List<HatData> _databaseCopy;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        HatPickup pickup = (HatPickup)target;

        if (_database != null)
        {
            bool displaying = false;

            for (int i = 0; i < _database.HatDatalist.Count; i++)
            {
                displaying = false;
                if(_database.HatDatalist[i].hatID == pickup.HatPickupID)
                {
                    DisplayCurrentHat(_database.HatDatalist[i]);
                    displaying = true;
                    break;
                }
            }

            if(!displaying)
            {
                DisplayNoHat(pickup.HatPickupID);
            }
        }
    }

    //helper functions for displaying hat ID information
    private void DisplayCurrentHat(HatData data)
    {
        GUI.color = Color.green;
        GUILayout.Label(data.hatName);
    }
    private void DisplayNoHat(int ID)
    {
        GUI.color = Color.red;
        GUILayout.Label("No Hat by ID: " + ID.ToString());
    }
}
