using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamageHazard : Hazard
{
    public float damagePerSecond = 1f;
    private GameManager gameManager;
    private bool playerTouching = false;
    private bool takingDamage = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !playerTouching && !takingDamage)
        {
            playerTouching = true;
            StartCoroutine(DamagePlayer());
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (playerTouching)
            playerTouching = false;
    }

    private IEnumerator DamagePlayer()
    {
        takingDamage = !takingDamage;
        InvokeDamage();
        gameManager.playerController.curHealth -= damagePerSecond;
        yield return new WaitForSeconds(1f);
        takingDamage = !takingDamage;
        if (playerTouching)
            StartCoroutine(DamagePlayer());
    }
}
