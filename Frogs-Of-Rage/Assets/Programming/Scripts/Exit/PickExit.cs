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

            if (exitPrefabs != null)
            {
                chosenExitPrefab = exitPrefabs[randomIndex];

                if (chosenExitPrefab != null)
                    chosenExitPrefab.SetActive(true);
                else
                    return;
            }

            Debug.Log("Chosen exit prefab: " + chosenExitPrefab.name);

            // Disable all canvas elements at start
            foreach (GameObject canvasElement in exitCanvasElements)
            {
                canvasElement.SetActive(false);
            }
        }
        else
        {
            Debug.Log("No exit prefabs found in the scene!");
        }
    }

    public void ShowExitCanvas()
    {
        // Enable only the canvas element that corresponds to the chosen exit prefab
        for (int i = 0; i < exitCanvasElements.Count; i++)
        {
            if (exitPrefabs[i] == chosenExitPrefab)
            {
                exitCanvasElements[i].SetActive(true);
            }
            else
            {
                exitCanvasElements[i].SetActive(false);
            }
        }
    }
}
