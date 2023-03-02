using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]

public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager gameManager = (GameManager)target;

        EditorGUILayout.Space(20);
        if (GUILayout.Button("Reset Collectable Count", GUILayout.Height(40)))
        {
            gameManager.ResetCollectables();
        }
    }
}
