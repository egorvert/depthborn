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
        if (!other.CompareTag("Player")) return; // If anything other than player touches, ignore
        if (CheckpointManager.Instance.SetCheckpoint(this))
        {
            PlayActivationEffect();
            PlayerBoost();
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

    public void PlayerBoost()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.speed += 1.0f; // increase speed 
            UnityEngine.Debug.Log("Player speed boosted!");
        }
    }
}
