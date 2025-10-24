using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Logic for moving the platform using paths.
public class MovingPlatform : MonoBehaviour {
    [SerializeField]
    private Keypath keypath;

    [SerializeField]
    private float speed;

    private int targetKeypathIndex;

    private Transform previousKeypath;
    private Transform targetKeypath;

    private float timeToKeypath;
    private float elapsedTime;

    // Preserve motion while on the platform 
    private Vector3 lastPosition;
    public Vector3 platformVelocity { get; private set; }

    void Start() {
        targetNextKeypath();
        lastPosition = transform.position;
    }

    void FixedUpdate() {
        elapsedTime += Time.deltaTime;

        // Smoothing to make player not jitter
        float elapsedPercentage = elapsedTime / timeToKeypath;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(previousKeypath.position, targetKeypath.position, elapsedPercentage);
        transform.rotation = Quaternion.Lerp(previousKeypath.rotation, targetKeypath.rotation, elapsedPercentage);

        platformVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;

        if (elapsedPercentage >= 1) {
            targetNextKeypath();
        }
    }

    // For multiple keypath movement, target the next in line
    private void targetNextKeypath() {
        previousKeypath = keypath.getKeypath(targetKeypathIndex);
        targetKeypathIndex = keypath.getNextKeypathIndex(targetKeypathIndex);
        targetKeypath = keypath.getKeypath(targetKeypathIndex);

        elapsedTime = 0;

        float distanceToKeypath = Vector3.Distance(previousKeypath.position, targetKeypath.position);
        timeToKeypath = distanceToKeypath / speed;
    }

    private void OnTriggerEnter(Collider other) {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null) {
            player.setOnPlatform(this);
        }
        
    }

    private void OnTriggerExit(Collider other) {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null) {
            player.setOnPlatform(null);
        }
    }
}
