using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamageHazard : Hazard
{
    public float damagePerSecond = 1f;
    private GameManager gameManager;
    private bool playerTouching = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTouching = true;
            StartCoroutine(DamagePlayer());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerTouching)
            playerTouching = !playerTouching;
    }

    private IEnumerator DamagePlayer()
    {
        InvokeDamage();
        gameManager.playerController.curHealth -= damagePerSecond;
        yield return new WaitForSeconds(1f);
        if(playerTouching)
            StartCoroutine(DamagePlayer());
    }
}
