using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundSettings))]
public class SoundSettingsEditor : Editor
{
    public AudioSource source;
    public override void OnInspectorGUI()
    {
        SoundSettings settings = (SoundSettings)target;

        source = (AudioSource)EditorGUILayout.ObjectField("Set Settings From Audio Source", source, typeof(AudioSource), true);

        if (source && GUILayout.Button("Set Settings from Source"))
            SetSettings(settings);



        base.OnInspectorGUI();
    }

    private void SetSettings(SoundSettings settings)
    {
        if (!source) return;

        SoundSettings.SoundSettingsData data = new SoundSettings.SoundSettingsData();

        data.priority = source.priority;
        data.volume = source.volume;
        data.pitch = source.pitch;
        data.spacialBlend = source.spatialBlend;

        data.minDistance = source.minDistance;
        data.maxDistance = source.maxDistance;
        data.spread = (int)source.spread;
        data.dopplerLevel = source.dopplerLevel;

        settings.SetSoundSettingsData(data);
        Debug.Log("Sound Settings: <b>" + settings.name + ("</b> set from <b>") + source.name + ("</b> Successfully"));
    }
}
