using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{

    private void OnSceneGUI()
    {
        AudioManager audioManager = (AudioManager)target;


        if(Application.isPlaying)
        {
            Handles.color = Color.yellow;
            VisualizePositionArray(audioManager.LowPrioritySFXPool, "Low Priority Sound Source");

            Handles.color = Color.red;
            VisualizePositionArray(audioManager.HighPrioritySFXPool, "High Priority Sound Source");

            Handles.color = Color.gray;
            VisualizePositionArray(audioManager.PlayerSFXPool, "Player Sound Source");

            if (audioManager.MusicSFXTransform.gameObject.activeSelf)
            {
                Handles.color = Color.blue;
                Handles.DrawSolidDisc(audioManager.MusicSFXTransform.position, Vector3.up, 1);
                Handles.Label(audioManager.MusicSFXTransform.position, "Music Sound Source");
            }
        }
    }

    private void VisualizePositionArray(Transform transformChildrenToDisplay, string testAtPosition)
    {
        for (int i = 0; i < transformChildrenToDisplay.childCount; i++)
        {
            if (transformChildrenToDisplay.GetChild(i).gameObject.activeSelf)
            {
                Handles.DrawSolidDisc(transformChildrenToDisplay.GetChild(i).transform.position, Vector3.up, 1);
                Handles.Label(transformChildrenToDisplay.GetChild(i).transform.position, testAtPosition);
            }
        }
    }
}
