using UnityEngine;

// Attach to an obstacle to slide it back and forth between its start position.
// and start position + offset.
public class Mover : MonoBehaviour
{
    public Vector3 offset = new Vector3(4f, 0f, 0f);
    public float speed = 0.5f;        // round trips get faster as this goes up.

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // PingPong returns a value that goes 0 -> 1 -> 0 -> 1 ...
        float t = Mathf.PingPong(Time.time * speed, 1f);
        transform.position = startPosition + offset * t;
    }
}
