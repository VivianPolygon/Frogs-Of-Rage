using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Sound Settings", menuName = "New Sound Settings")]
public class SoundSettings : ScriptableObject
{
    [Range(0, 256)] public int priority = 128; 
    [Range(0, 1)] public float volume = 1; 
    [Range(-3, 3)] public float pitch = 1;   
    [Range(0, 1)] public float spacialBlend = 1;
 
    [Min(0)] public float minDistance = 1; 
    [Min(0)] public float maxDistance = 500; 
    [Range(0, 360)] public int spread = 0;
    [Range(0, 5)] public float dopplerLevel = 1;


    //struct for all the data
    public struct SoundSettingsData
    {
        [Range(0, 256)] public int priority;
        [Range(0, 1)] public float volume;
        [Range(-3, 3)] public float pitch;
        [Range(0, 1)] public float spacialBlend;

        [Min(0)] public float minDistance;
        [Min(0)] public float maxDistance;
        [Range(0, 360)] public int spread;
        [Range(0, 5)] public float dopplerLevel;
    }
    //sets the data, used for the editor script
    public void SetSoundSettingsData(SoundSettingsData data)
    {
        priority = data.priority;
        volume = data.volume;
        pitch = data.pitch;
        spacialBlend = data.spacialBlend;

        minDistance = data.minDistance;
        maxDistance = data.maxDistance;
        spread = data.spread;
        dopplerLevel = data.dopplerLevel;
    }
}
