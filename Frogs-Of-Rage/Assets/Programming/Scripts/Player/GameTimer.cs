using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public float totalTime = 0f;
    public float minutes;
    public float seconds;
    public float milliseconds;

    private void Update()
    {
        totalTime += Time.deltaTime;

        minutes = Mathf.FloorToInt(totalTime / 60);
        seconds = Mathf.FloorToInt(totalTime % 60);
        milliseconds = Mathf.FloorToInt(totalTime * 1000) % 1000;


    }
}
