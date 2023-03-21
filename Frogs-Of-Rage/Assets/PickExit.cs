using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickExit : MonoBehaviour
{
    public List<GameObject> exitPrefabs;

    private void Start()
    {
        exitPrefabs = new List<GameObject>();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Exit"))
            {
                exitPrefabs.Add(obj);
            }
        }

        if (exitPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, exitPrefabs.Count);
            for (int i = 0; i < exitPrefabs.Count; i++)
            {
                if (i == randomIndex)
                {
                    // Enable the chosen exit prefab
                    exitPrefabs[i].SetActive(true);
                }
                else
                {
                    // Disable the other exit prefabs
                    exitPrefabs[i].SetActive(false);
                }
            }
            Debug.Log("Chosen exit prefab: " + exitPrefabs[randomIndex].name);
        }
        else
        {
            Debug.Log("No exit prefabs found in the scene!");
        }
    }
}
