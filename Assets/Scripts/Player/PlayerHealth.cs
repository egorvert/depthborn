using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlayerHealth : MonoBehaviour {
    public Rigidbody rb;
    public PlayerMovement pm;
    
    private PlayerInventory playerInventory;
	  private float lastDeathTime;

    void Start() {
        rb = GetComponent<Rigidbody>();
		    playerInventory = GetComponent<PlayerInventory>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Fish") && Time.time > lastDeathTime + 0.1f)
        {
            UnityEngine.Debug.Log("ONO U DED! :(((");
            Die();
        }
    }

    // --- Added: death/respawn ---
    public void Die() {
        lastDeathTime = Time.time;
		playerInventory.IncrementDeath();
        
        Debug.Log("Respawning...");
        // Put death VFX/SFX/animation hooks here in the future

        Respawn();
    }

    private void Respawn() {
        // Find the last checkpoint activated (falls back to starting checkpoint or Vector3.zero)
        Vector3 pos = CheckpointManager.Instance.GetRespawnPosition();
        Quaternion rot = CheckpointManager.Instance.GetRespawnRotation();
        UnityEngine.Debug.Log($"Respawning to {pos} with rotation {rot}");

        // Reset physics before teleport to avoid carry-over momentum
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = pos;
        transform.rotation = rot;
    
        // Reset oxygen to max
        if (pm)
            pm.SetOxygen(pm.maxOxygen);
    }

}
