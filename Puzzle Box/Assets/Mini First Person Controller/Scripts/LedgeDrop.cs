using UnityEngine;

public class LedgeDrop : MonoBehaviour
{
    private bool isTriggered;
    private float dropTimer;
    private float respawnTimer;
    private Vector3 originalPos;
    private bool hasDropped;

    void Start()
    {
        originalPos = transform.position;
        Debug.Log("Ledge starting at: " + originalPos);
    }

    void Update()
    {
        if (isTriggered)
        {
            Debug.Log("Drop timer: " + dropTimer);
            dropTimer -= Time.deltaTime;
            if (dropTimer <= 0 && !hasDropped)
            {
                transform.position = new Vector3(transform.position.x, -5f, transform.position.z);
                Debug.Log("Ledge dropped to: " + transform.position);
                hasDropped = true;
            }
        }

        if (hasDropped)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0)
            {
                // Reset ledge
                transform.position = originalPos;
                isTriggered = false;
                hasDropped = false;
                Debug.Log("Ledge respawned at: " + originalPos);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            Debug.Log("Player stepped on ledge!");
            isTriggered = true;
            dropTimer = 3f;
            respawnTimer = 1f; // Respawn after 3 seconds
        }
    }
}