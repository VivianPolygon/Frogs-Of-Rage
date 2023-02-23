using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float totalTime = 0f;
    public float minutes;
    public float seconds;

    private void Update()
    {
        totalTime += Time.deltaTime;

        minutes = Mathf.FloorToInt(totalTime / 60);
        seconds = Mathf.FloorToInt(totalTime % 60);


    }
}
