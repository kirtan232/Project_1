using UnityEngine;

// Attach to an obstacle to make it rotate continuously (e.g. a sweeping bar).
public class Spinner : MonoBehaviour
{
    public Vector3 degreesPerSecond = new Vector3(0f, 90f, 0f);

    void Update()
    {
        transform.Rotate(degreesPerSecond * Time.deltaTime);
    }
}
