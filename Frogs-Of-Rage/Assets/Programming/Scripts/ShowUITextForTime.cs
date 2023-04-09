using UnityEngine;
using UnityEngine.UI;

public class ShowUITextForTime : MonoBehaviour
{
    public float displayTime = 3f;
    public Text uiText;
    private bool hasCollided = false;
    private bool audioStarted = false;
    private PlayAudioOnTrigger audioTrigger;

    void Start()
    {
        if (uiText != null)
        {
            uiText.enabled = false;
        }
        audioTrigger = GetComponent<PlayAudioOnTrigger>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            hasCollided = true;
            if (audioTrigger.hasPlayed)
            {
                audioStarted = true;
                ShowUIText();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            hasCollided = false;
            if (uiText != null)
            {
                uiText.enabled = false;
            }
            audioStarted = false;
        }
    }

    void Update()
    {
        if (hasCollided && Input.GetKeyDown(KeyCode.E) && audioTrigger.hasPlayed)
        {
            if (!audioStarted)
            {
                audioStarted = true;
                ShowUIText();
            }
        }
    }

    void ShowUIText()
    {
        if (uiText != null)
        {
            uiText.enabled = true;
        }
        Invoke("HideUIText", displayTime);
    }

    void HideUIText()
    {
        if (uiText != null)
        {
            uiText.enabled = false;
        }
        audioStarted = false;
    }
}
