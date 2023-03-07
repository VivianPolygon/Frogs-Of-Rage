using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    public Vector3 rotationSpeed;
    private void Update()
    {
        Vector3 caculatedRotSpeed = rotationSpeed * Time.deltaTime;

        transform.Rotate(caculatedRotSpeed);
    }
}
