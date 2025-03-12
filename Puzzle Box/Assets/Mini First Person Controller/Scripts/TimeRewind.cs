using UnityEngine;
using System.Collections.Generic;

public class TimeRewind : MonoBehaviour
{
   

    // Configurable settings
    public float maxRewindTime = 8f; // 8 seconds of rewind for experimentation
    public KeyCode rewindKey = KeyCode.R; // Hold 'R' to rewind
    public ParticleSystem rewindEffect; // Optional: Add a glitchy particle effect
  

    // Internal data
    private List<PlayerState> recordedStates = new List<PlayerState>();
    private bool isRewinding = false;
    private FirstPersonMovement fpController; // Replace FirstPersonController

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
        // Hold to rewind, release to stop
        if (Input.GetKeyDown(rewindKey))
        {
            StartRewind();
        }
        if (Input.GetKeyUp(rewindKey))
        {
            StopRewind();
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
            transform.position = Vector3.Lerp(transform.position, state.position, Time.fixedDeltaTime * 10f); // Smooth transition
            transform.rotation = Quaternion.Slerp(transform.rotation, state.rotation, Time.fixedDeltaTime * 10f); // Smooth rotation
            recordedStates.RemoveAt(recordedStates.Count - 1); // Remove used state
        }
        else
        {
            StopRewind(); // Stop when out of states
        }
    }

    void StartRewind()
    {
        if (recordedStates.Count > 0) // Only rewind if there's something to rewind to
        {
            lastPositionBeforeRewind = transform.position; // Saves Posistions
            lastRotationBeforeRewind = transform.rotation; // Saves Positions
            isRewinding = true;
            fpController.enabled = false; // Disable movement
            if (rewindEffect != null) rewindEffect.Play(); // Start glitch effect
        }
    }

    void StopRewind()
    {
        isRewinding = false;
        fpController.enabled = true; // Re-enable movement
        if (rewindEffect != null) rewindEffect.Stop(); // Stop glitch effect
    }
}