using UnityEngine;
using System.Collections.Generic;

public class MusicPlayer : MonoBehaviour
{
    public List<AudioClip> musicClips;
    private AudioSource audioSource;
    private int currentClipIndex;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomClip();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            currentClipIndex++;
            if (currentClipIndex >= musicClips.Count)
            {
                currentClipIndex = 0;
            }
            audioSource.clip = musicClips[currentClipIndex];
            audioSource.Play();
        }
    }

    void PlayRandomClip()
    {
        currentClipIndex = Random.Range(0, musicClips.Count);
        audioSource.clip = musicClips[currentClipIndex];
        audioSource.Play();
    }
}
