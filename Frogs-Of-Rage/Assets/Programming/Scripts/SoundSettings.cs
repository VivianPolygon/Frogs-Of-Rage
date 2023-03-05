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
}
