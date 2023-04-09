using UnityEngine;
using System.Collections.Generic;

public class MusicPlayer : MonoBehaviour
{
    public List<AudioClip> musicClips;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomClip();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayRandomClip();
        }
    }

    void PlayRandomClip()
    {
        int randomIndex = Random.Range(0, musicClips.Count);
        audioSource.clip = musicClips[randomIndex];
        audioSource.Play();
    }
}
