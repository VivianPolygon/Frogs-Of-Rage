using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private string soundSettings;

    private int soundKey;

    private void Start()
    {
        //InvokeRepeating("PlayClip", Random.Range(0f, 0.9f), 1f);
        PlayClip();
        AudioManager.SetAdjustableAudioLooping(soundKey, true);
    }

    private void PlayClip()
    {
        soundKey = AudioManager.Instance.PlayAdjustableAudio(transform, SoundType.LowPrioritySoundEffect, soundSettings, clip, false);
    }
}
