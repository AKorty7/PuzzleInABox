using UnityEngine;
using System.Collections.Generic;

public class TimeRewind : MonoBehaviour
{
   

    // Configurable settings
    public float maxRewindTime = 8f; // 8 seconds of rewind for experimentation
    public KeyCode rewindKey = KeyCode.R; // Hold 'R' to rewind
    public ParticleSystem rewindEffect; // Optional: Add a glitchy particle effect
    public float ghostLifetime = 3f; // How long ghost lasts in seconds
    private float ghostTimer; // Tracks time left
    public float rewindCooldown = 5f; // Cooldown time in seconds
    private float cooldownTimer; // Tracks time left until next rewind


    // Internal data
    private List<PlayerState> recordedStates = new List<PlayerState>();
    private bool isRewinding = false;
    private FirstPersonMovement fpController; // Replace FirstPersonController
    public UnityEngine.UI.Image cooldownIcon; // Assign in Inspector

    // Where the player’s ghost shows from
    private Vector3 lastPositionBeforeRewind; // Stores the player's position before rewinding
    private GameObject currentMarker; // Keeps track of the spawned marker
    private Quaternion lastRotationBeforeRewind; // To store the player's facing direction


   
    // Store position, rotation, and timestamp
    private class PlayerState
    {
        public Vector3 position;
        public Quaternion rotation;
        public float timeStamp;

        public PlayerState(Vector3 pos, Quaternion rot, float time)
        {
            position = pos;
            rotation = rot;
            timeStamp = time;
        }
    }

    void Start()
    {
        fpController = GetComponent<FirstPersonMovement>(); // Adjust to your script
        if (rewindEffect != null) rewindEffect.Stop(); // Ensure effect is off at start
    }

    void FixedUpdate()
    {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(rewindKey))
        {
            StartRewind();
        }
        if (Input.GetKeyUp(rewindKey))
        {
            StopRewind();
        }
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        if (cooldownIcon != null)
        {
            cooldownIcon.fillAmount = cooldownTimer / rewindCooldown; // 1 to 0 as it counts down
        }
        if (currentMarker != null)
        {
            Debug.Log("Ghost timer: " + ghostTimer); // Keep for now—remove later if you want
            ghostTimer -= Time.deltaTime;
            if (ghostTimer <= 0)
            {
                Destroy(currentMarker);
                currentMarker = null;
            }
        }
    }

    void Record()
    {
        // Record position and rotation every fixed frame
        recordedStates.Add(new PlayerState(transform.position, transform.rotation, Time.time));

        // Trim states older than 8 seconds
        while (recordedStates.Count > 0 && (Time.time - recordedStates[0].timeStamp) > maxRewindTime)
        {
            recordedStates.RemoveAt(0);
        }
    }

    void Rewind()
    {
        if (recordedStates.Count > 0)
        {
            // Get the latest state
            PlayerState state = recordedStates[recordedStates.Count - 1];

            // Smoothly move to the position and rotation
            transform.position = Vector3.Lerp(transform.position, state.position, Time.fixedDeltaTime * 20f); // Smooth transition
            transform.rotation = Quaternion.Slerp(transform.rotation, state.rotation, Time.fixedDeltaTime * 20f); // Smooth transition
            recordedStates.RemoveAt(recordedStates.Count - 1); // Remove used state
        }
        else
        {
            StopRewind(); // Stop when out of states
        }
    }

    void StartRewind()
    {
        if (recordedStates.Count > 0 && cooldownTimer <= 0)
        {
            if (currentMarker != null)
            {
                Destroy(currentMarker);
                currentMarker = null;
            }
            lastPositionBeforeRewind = transform.position;
            lastRotationBeforeRewind = transform.rotation;
            if (currentMarker == null)
            {
                currentMarker = Instantiate(gameObject, lastPositionBeforeRewind, lastRotationBeforeRewind);
                Camera cloneCamera = currentMarker.GetComponentInChildren<Camera>();
                if (cloneCamera != null) cloneCamera.enabled = false;
                TimeRewind cloneRewind = currentMarker.GetComponent<TimeRewind>();
                if (cloneRewind != null) cloneRewind.enabled = false;
                foreach (MonoBehaviour script in currentMarker.GetComponents<MonoBehaviour>())
                {
                    script.enabled = false;
                }
                foreach (Renderer rend in currentMarker.GetComponentsInChildren<Renderer>())
                {
                    Material mat = rend.material;
                    mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.5f);
                }
                ghostTimer = ghostLifetime;
            }
            Debug.Log("Recorded states count: " + recordedStates.Count);
            PlayerState oldestState = recordedStates[0];
            Debug.Log("Snapping to: " + oldestState.position);
            transform.position = oldestState.position;
            transform.rotation = oldestState.rotation;
            recordedStates.Clear();
            cooldownTimer = rewindCooldown;
            fpController.enabled = false;
            if (rewindEffect != null) rewindEffect.Play();
        }
    }

    void StopRewind()
    {
        fpController.enabled = true;
        if (rewindEffect != null) rewindEffect.Stop();
    }
}