using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioProximityTest : MonoBehaviour
{
    [SerializeField] private AudioClip _clip;


    private void Awake()
    {
        InvokeRepeating("PlaySound", 1, 1);
    }

    private void PlaySound()
    {
        AudioManager.Instance.PlaySoundEffect(transform, SoundType.LowPrioritySoundEffect, "Sound Settings 1", _clip);
    }

    private void OnGUI()
    {
        GUILayout.Box("Select the audio manager object in the heirarchy to see the proximity debug info in the scene on runtime");
        GUILayout.Box("It's under Don't Destroy on Load");
    }
}
