using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOpeningExit : MonoBehaviour
{
    public bool openingExit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.Instance.playerController.secondaryObjectiveComplete)
        {
            GameObject.FindGameObjectWithTag("Exit").GetComponent<Collider>().enabled = true;
            openingExit = true;
            GetComponent<Animator>().SetTrigger("ObjectiveComplete");
        }
    }
}
