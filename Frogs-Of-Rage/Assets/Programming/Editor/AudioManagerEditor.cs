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
    private GameObject _customObject;

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


        _proxMode = audioManager.ProxMode;
        _cullingQuality = audioManager.CullingQuality;

        if(_customObject != null)
            _customObject = audioManager.CustomObjectTransform.gameObject;


        if (audioManager.ProximityPrioritization)
        {
            _cullingQuality = (AudioManager.AudioCullingQuality)EditorGUILayout.EnumPopup("Culling Quality", _cullingQuality);
            audioManager.SetQuality(_cullingQuality);

            _proxMode = (AudioManager.ProximityMode)EditorGUILayout.EnumPopup("Proximity Mode", _proxMode);
            audioManager.SetProximityMode(_proxMode);

            if(_proxMode == AudioManager.ProximityMode.CustomObject)
            {
                _customObject = (GameObject)EditorGUILayout.ObjectField("Custom Object Transform", _customObject, typeof(Object), true);

                if (_customObject == null)
                {
                    return;
                }

                audioManager.CustomObjectTransform = _customObject.transform;
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
