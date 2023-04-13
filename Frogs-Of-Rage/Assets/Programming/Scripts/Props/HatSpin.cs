using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatSpin : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public bool useWorldUp = false;

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, useWorldUp ? Space.World : Space.Self);
    }
}
