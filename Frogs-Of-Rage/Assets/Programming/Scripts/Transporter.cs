using UnityEngine;
using Unity.Collections;
using static GameManager;
using System.Collections.Generic;
using System.Collections;

public class Transporter : MonoBehaviour
{
    public Transform destination;
    public GameObject TutRoom;

    public PickExit pickExit;

    private void Start()
    {
        //pickExit = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PickExit>();
        pickExit = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<PickExit>();

        if (destination == null)
        {
            Debug.LogWarning("Transporter: No destination assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Check if the destination is assigned
            if (destination != null)
            {
                other.gameObject.transform.position = destination.position;








                // Hide all exit canvas elements before showing the chosen one
                pickExit.HideAllExitCanvas();

                // Enable the chosen exit canvas element
                pickExit.ShowExitCanvas();
            }
            else
            {
                // Warn the developer that the destination is not assigned
                Debug.LogWarning("Transporter: No destination assigned!");
            }

            // Disable this game object after teleporting the player
            gameObject.SetActive(false);
            TutRoom.SetActive(false);
        }
    }



    private IEnumerator WaitForFadeScreen()
    {
        UIManager.Instance.ChangeState(CanvasState.Death);
        UIManager.Instance.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(3.2f);
        OnPlayerDeath?.Invoke(new PlayerDeathEventArgs(lastCheckpointPos));

    }
}
