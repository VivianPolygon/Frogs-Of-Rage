using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickExit : MonoBehaviour
{
    public List<GameObject> exitPrefabs;
    public List<GameObject> exitCanvasElements;

    private GameObject chosenExitPrefab;

    private void Start()
    {
        if (exitPrefabs.Count == 0)
        {
            Debug.LogError("No exit prefabs assigned to the PickExit script!");
            return;
        }

        if (exitPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, exitPrefabs.Count);
            chosenExitPrefab = exitPrefabs[randomIndex];
            chosenExitPrefab.SetActive(true);

            Debug.Log("Chosen exit prefab: " + chosenExitPrefab.name);

            for (int i = 0; i < exitPrefabs.Count; i++)
            {
                GameObject exit = exitPrefabs[i];
                GameObject canvasElement = exitCanvasElements[i];

                if (exit == chosenExitPrefab)
                {
                   
                    canvasElement.SetActive(true);
                }
                else
                {
                    exit.SetActive(false);
                    canvasElement.SetActive(false);
                }

                //Text exitText = canvasElement.GetComponent<Text>();

                //if (exitText != null)
                //{
                //    Debug.Log("Is this working?");
                //    exitText.text = exit.name;
                //}
                //else
                //{
                //    Debug.LogError("No Text component found on the canvas element for exit " + exit.name);
                //}
            }
        }
        else
        {
            Debug.Log("No exit prefabs found in the scene!");
        }
    }
}
