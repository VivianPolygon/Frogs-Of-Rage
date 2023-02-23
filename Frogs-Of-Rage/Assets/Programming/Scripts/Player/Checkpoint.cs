using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    private GameManager gameManager;


    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    private void Update()
    {
        Rotate();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gameManager.lastCheckpointPos = transform.position;
            //Debug.Log(gameManager.lastCheckpointPos);
            UIManager.Instance.checkpointIcon.GetComponent<Animator>().SetTrigger("OnCheckpoint");
        }
    }


    public void Rotate()
    {
        transform.Rotate(0, 1, 0, Space.World);
    }

}
