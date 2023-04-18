using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardAudio : MonoBehaviour
{
    public List<AudioClip> keyStrokes;
    private AudioSource audioSource;
    public string playerTag = "Player";
    private bool isPlaying = false;
    private GameObject player;
    private Vector3 lastPlayerPosition;
    private float movementThreshold = 0.1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag(playerTag);
        lastPlayerPosition = player.transform.position;
    }

    void Update()
    {
        if (PlayerIsMoving() && isPlaying)
        {
            PlayRandomKeyStroke();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlaying = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlaying = false;
        }
    }

    private bool PlayerIsMoving()
    {
        Vector3 currentPlayerPosition = player.transform.position;
        float distanceMoved = Vector3.Distance(lastPlayerPosition, currentPlayerPosition);

        lastPlayerPosition = currentPlayerPosition;
        return distanceMoved > movementThreshold;
    }

    private void PlayRandomKeyStroke()
    {
        if (!audioSource.isPlaying)
        {
            int randomIndex = Random.Range(0, keyStrokes.Count);
            audioSource.clip = keyStrokes[randomIndex];
            audioSource.Play();
        }
    }
}
