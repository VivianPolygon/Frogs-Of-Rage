using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitEffectturnoff : MonoBehaviour
{
    // Travis Made this >.> 
    //If it breaks anything it wasn't made by Travis
    //Destorys MissionEffect on player collison 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           Destroy(gameObject);

        }
    }
    //Lol
}
