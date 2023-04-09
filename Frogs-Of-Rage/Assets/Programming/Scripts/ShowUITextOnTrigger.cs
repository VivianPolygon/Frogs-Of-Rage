using UnityEngine;
using UnityEngine.UI;

public class ShowUITextOnTrigger : MonoBehaviour
{
    public Text uiText;
    //private bool hasCollided = false;

    void Start()
    {
        if (uiText != null)
        {
            uiText.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //hasCollided = true;
            if (uiText != null)
            {
                uiText.enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //hasCollided = false;
            if (uiText != null)
            {
                uiText.enabled = false;
            }
        }
    }
}
