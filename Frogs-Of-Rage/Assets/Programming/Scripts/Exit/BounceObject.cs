using UnityEngine;

public class BounceObject : MonoBehaviour
{
    public float bounceHeight = 1f;
    public float bounceSpeed = 1f;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * bounceHeight;
    }

    void Update()
    {
        float time = Mathf.PingPong(Time.time * bounceSpeed, 1);
        transform.position = Vector3.Lerp(startPosition, targetPosition, time);
    }
}
