using UnityEngine;

// Trigger for player so they can move WITH the platform instead of float stationary
public class PlatformTrigger : MonoBehaviour {
    private MovingPlatform platform;

    void Start() {
        platform = GetComponentInParent<MovingPlatform>();
    }

    private void OnTriggerEnter(Collider other) {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null) {
            player.setOnPlatform(platform); // sets the platform
        }
    }

    private void OnTriggerExit(Collider other) {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null) {
            player.setOnPlatform(null); // removes platform
        }
    }
}