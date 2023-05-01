using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NewOptionMenuSettings", menuName ="Option Menu Settings")]
public class OptionsMenuSettings : ScriptableObject
{
    [Header("Player Seen Ranges")]
    public SettingsValueRange sfxSliderRange;
    public SettingsValueRange musicSliderRange;
    public SettingsValueRange mouseSensitivitySliderRange;

    [Header("Translated Internal Ranges")]
    [Space(20)]
    public SettingsValueRange appliedSFXSliderRange;
    public SettingsValueRange appliedMusicSliderRange;
    public SettingsValueRange appliedMouseSensitivitySliderRange;
}


//editor script used to flip values if a higher value is inputed into min. reduces human error.
#if UNITY_EDITOR
[CustomEditor(typeof(OptionsMenuSettings))]
public class OptionsMenuSettingsEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OptionsMenuSettings settings = (OptionsMenuSettings)target;


        settings.sfxSliderRange.TryFlipValues();
        settings.musicSliderRange.TryFlipValues();
        settings.mouseSensitivitySliderRange.TryFlipValues();

        settings.appliedSFXSliderRange.TryFlipValues();
        settings.appliedMusicSliderRange.TryFlipValues();
        settings.appliedMouseSensitivitySliderRange.TryFlipValues();
    }


}
#endif
