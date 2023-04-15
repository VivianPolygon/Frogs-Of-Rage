using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardAudio : MonoBehaviour
{
    public List<AudioClip> keyStrokes;
    private AudioSource audioSource;
    private bool isPlaying = false;
    private PlayerState playerState;

    public enum PlayerState
    {
        Idle,
        Walking,
        Sprinting
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isPlaying && !audioSource.isPlaying)
        {
            PlayRandomKeyStroke();
        }
    }

    public void SetPlayerState(PlayerState state)
    {
        playerState = state;

        if (playerState == PlayerState.Walking || playerState == PlayerState.Sprinting)
        {
            isPlaying = true;
            if (!audioSource.isPlaying)
            {
                PlayRandomKeyStroke();
            }
        }
        else
        {
            isPlaying = false;
        }
    }

    private void PlayRandomKeyStroke()
    {
        int randomIndex = Random.Range(0, keyStrokes.Count);
        audioSource.clip = keyStrokes[randomIndex];
        audioSource.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Play();
        }
    }
}
