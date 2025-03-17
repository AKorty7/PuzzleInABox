using UnityEngine;
using TMPro;

public class ButtonTrigger : MonoBehaviour
{
    public GameObject door;
    public TextMeshProUGUI interactPrompt;
    private bool isPressed;
    private float doorTimer;
    private Vector3 doorClosedPos;
    private Vector3 doorOpenPos;
    private bool playerInRange;

    void Start()
    {
        doorClosedPos = door.transform.position;
        doorOpenPos = doorClosedPos + new Vector3(0, 10, 0);
        if (interactPrompt != null) interactPrompt.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPressed)
        {
            doorTimer -= Time.deltaTime;
            door.transform.position = doorOpenPos;
            if (doorTimer <= 0)
            {
                isPressed = false;
                door.transform.position = doorClosedPos;
            }
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isPressed)
        {
            Debug.Log("E pressed!");
            isPressed = true;
            doorTimer = 3f;
            door.transform.position = doorOpenPos;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger!");
            playerInRange = true;
            if (interactPrompt != null) interactPrompt.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger!");
            playerInRange = false;
            if (interactPrompt != null) interactPrompt.gameObject.SetActive(false);
        }
    }
}