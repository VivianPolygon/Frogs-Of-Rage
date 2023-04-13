using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HatDatabase))]
public class DatabaseEditorViewer : Editor
{

    public override void OnInspectorGUI()
    {
        HatDatabase database = (HatDatabase)target;

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < database.HatDatalist.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Hat Name : " + database.HatDatalist[i].hatName);
            EditorGUILayout.LabelField("ID: " + database.HatDatalist[i].hatID.ToString());
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

}
