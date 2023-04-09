using UnityEngine;

public class HealingPool : MonoBehaviour
{
    [SerializeField] private float healAmount = 10f;

    private PlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player != null)
            {
                //float healthDiff = player.GetMaxHealth() - player.GetCurrentHealth();
                //float actualHealAmount = Mathf.Min(healAmount * Time.deltaTime, healthDiff);
                //player.Heal(actualHealAmount);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }
}
