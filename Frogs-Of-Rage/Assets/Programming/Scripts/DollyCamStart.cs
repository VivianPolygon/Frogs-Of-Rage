using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyCamStart : MonoBehaviour
{
    private Animator camControllerAnimator;

    [Tooltip("Set this string to the same name as the dolly cam you want to start on trigger enter.")]
    public string dollyCamName;


    private void Start()
    {
        camControllerAnimator = GameObject.Find("CM StateDrivenCamera1").GetComponent<Animator>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //Trigger animations
            camControllerAnimator.SetTrigger(dollyCamName);
            //Disable trigger collider
            Collider boxCollider = GetComponent<BoxCollider>();
            boxCollider.enabled = false;
        }
    }
}
