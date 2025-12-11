using UnityEngine;

public class QuickSand : MonoBehaviour
{
    [Header("Sinking Settings")]
    [Tooltip("How fast the player sinks down while in the sand.")]
    public float sinkSpeed = 0.8f;            // Affects sinking speed. Higher = faster sink.

    [Tooltip("Slow upward assist when holding jump inside sand.")]
    public float escapeAssist = 3f;           // Affects how quickly the player rises while spamming jump. Higher = easier struggle.

    [Tooltip("Slight nudge upward when exiting sand to prevent sticking.")]
    public float liftNudge = 0.06f;           // Not affecting speed much, just lifts player slightly on exit.

    [Tooltip("Distance above sand surface at which auto-exit happens if holding jump.")]
    public float exitThreshold = 0.18f;       // Affects how high player must rise inside sand to trigger exit.

    [Tooltip("Depth at which the player will fall through the sand if fully submerged.")]
    public float fullySubmergedY = -1.5f;     // Affects when player falls through if sinking too deep.

    [Tooltip("Velocity applied when forcing the player to jump out of sand.")]
    public float exitJumpVelocity = 6f;       // Affects the height of the jump when leaving the sand. Higher = higher exit jump.

    private Rigidbody playerRb;
    private Transform playerTransform;
    private float startY;
    private bool inSand = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerRb = other.GetComponent<Rigidbody>();
        if (playerRb == null) return;

        playerTransform = other.transform;
        inSand = true;
        startY = playerTransform.position.y;

        // Stop gravity and reset vertical velocity so player doesn't slam into the sand
        playerRb.useGravity = false;
        Vector3 v = playerRb.velocity;
        v.y = 0f;
        playerRb.velocity = v;

        // Apply drag to slow horizontal movement inside sand
        playerRb.drag = 6f;
    }

    void OnTriggerStay(Collider other)
    {
        if (!inSand || !other.CompareTag("Player") || playerRb == null) return;

        // --- SINKING ---
        playerRb.AddForce(Vector3.down * sinkSpeed, ForceMode.Acceleration); // This controls how fast the player sinks

        // --- STRUGGLING UP ---
        if (Input.GetButton("Jump"))
        {
            playerRb.AddForce(Vector3.up * escapeAssist, ForceMode.Acceleration); // This affects rise speed while holding jump

            // Auto-exit if player rises above threshold
            if (playerTransform.position.y >= startY + exitThreshold)
            {
                ForceExit();
                return;
            }
        }

        // Single jump press forces exit immediately
        if (Input.GetButtonDown("Jump"))
        {
            ForceExit();
            return;
        }

        // Fall through if too deep
        if (playerTransform.position.y < startY + fullySubmergedY)
        {
            RestorePhysics();
            inSand = false;
            return;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        RestorePhysics();
        inSand = false;
    }

    // --- Handles exit logic and triggers forced jump ---
    private void ForceExit()
    {
        if (!inSand || playerRb == null) return;

        inSand = false;

        // Slight nudge above sand surface
        playerTransform.position += Vector3.up * liftNudge;

        // Restore gravity and drag before forcing jump
        playerRb.useGravity = true;
        playerRb.drag = 0f;

        // Reset vertical velocity so we can control exit jump height precisely
        Vector3 vel = playerRb.velocity;
        vel.y = 0f;
        playerRb.velocity = vel;

        // --- EXIT JUMP ---
        // This controls the jump height when leaving the sand
        PlayerMovement pm = playerRb.GetComponent<PlayerMovement>();
        if (pm != null)
            pm.ForceJump(exitJumpVelocity); // exitJumpVelocity determines how high the jump is
    }

    private void RestorePhysics()
    {
        if (playerRb == null) return;
        playerRb.useGravity = true;
        playerRb.drag = 0f;
    }
}