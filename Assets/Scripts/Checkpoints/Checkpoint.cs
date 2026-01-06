using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint; // If null, uses this transform
    public ParticleSystem activationEffect;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (CheckpointManager.Instance.SetCheckpoint(this))
        {
            PlayActivationEffect();
        
            // Increase player speed and reset oxygen
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm)
            {
                pm.IncreaseSpeed();
                pm.SetOxygen(pm.maxOxygen);
            }
        }
        UnityEngine.Debug.Log("Checkpoint set!");
    }

    public Vector3 GetPosition() => (respawnPoint ? respawnPoint : transform).position;
    public Quaternion GetRotation() => (respawnPoint ? respawnPoint : transform).rotation;

    public void PlayActivationEffect()
    {
        if (activationEffect)
        {
            activationEffect.Play();
        }
    }
}
