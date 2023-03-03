using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    private AudioManager.ProximityMode _proxMode;
    private AudioManager.AudioCullingQuality _cullingQuality;


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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AudioManager audioManager = (AudioManager)target;

        if(audioManager.ProximityPrioritization)
        {
            _proxMode = (AudioManager.ProximityMode)EditorGUILayout.EnumPopup("Proximity Mode", _proxMode);
            audioManager.SetProximityMode(_proxMode);

            _cullingQuality = (AudioManager.AudioCullingQuality)EditorGUILayout.EnumPopup("Culling Quality", _cullingQuality);
            audioManager.SetQuality(_cullingQuality);
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
