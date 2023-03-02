using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private string soundSettings;

    private void Start()
    {
        InvokeRepeating("PlayClip", Random.Range(0f, 0.9f), 1f);
    }

    private void PlayClip()
    {
        AudioManager.Instance.PlaySoundEffect(transform.position, SoundType.LowPrioritySoundEffect, soundSettings, clip);
    }
}
