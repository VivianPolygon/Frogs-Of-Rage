using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hazard : MonoBehaviour
{
    public float range;
    public float damage = 1;
    public float waitTime = 1f;

    public static event Action OnDamage;
    public static void InvokeDamage() { OnDamage?.Invoke(); }

    [HideInInspector]
    public bool canDamage = true;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;

        //StartCoroutine(CheckDistance());
    }

    private void Update()
    {
        if(canDamage)
        {
            StartCoroutine(CheckDistance());
        }
    }


    private IEnumerator CheckDistance()
    {
        canDamage = false;
        if (Vector3.Distance(transform.position, gameManager.playerController.transform.position) <= range)
        {
            InvokeDamage();
            gameManager.playerController.curHealth -= damage;
        }
        yield return new WaitForSeconds(waitTime);
        canDamage = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
