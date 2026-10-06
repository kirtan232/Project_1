using UnityEngine;

// Attach to every coin. The coin's collider must have "Is Trigger" checked.
public class Coin : MonoBehaviour
{
    public float spinSpeed = 120f;    // degrees per second

    bool collected = false;

    void Start()
    {
        GameManager.Instance.RegisterCoin();
    }

    void Update()
    {
        // Spin around the world's up axis
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
    }

    // Unity calls this when another collider enters this trigger
    void OnTriggerEnter(Collider other)
    {
        if (collected) return;                    // never count the same coin twice
        if (!other.CompareTag("Player")) return;

        collected = true;
        GameManager.Instance.CollectCoin();
        Destroy(gameObject);                      // the coin disappears
    }
}
