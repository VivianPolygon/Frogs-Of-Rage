using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayAudioOnTrigger : MonoBehaviour
{
    public AudioClip audioClip;
    public Text interactText;
    private AudioSource audioSource;
    public bool hasPlayed = false;
    private static AudioSource currentAudioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.playOnAwake = false;
        interactText.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !hasPlayed)
        {
            interactText.enabled = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && !hasPlayed && Input.GetKeyDown(KeyCode.E))
        {
            if (currentAudioSource != null)
            {
                currentAudioSource.Stop();
            }
            StartCoroutine(PlayAudioWithDelay(0.1f)); // Add a small delay before playing the audio
            interactText.enabled = false;
        }

        if (currentAudioSource == audioSource && !audioSource.isPlaying)
        {
            hasPlayed = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !hasPlayed)
        {
            interactText.enabled = false;
        }
    }

    private IEnumerator PlayAudioWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
        currentAudioSource = audioSource;
        hasPlayed = true;
    }
}
