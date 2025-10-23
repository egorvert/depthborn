using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [Tooltip("Initial checkpoint for the level")]
    public Checkpoint startingCheckpoint;

    public Checkpoint CurrentCheckpoint { get; private set; }

    private void Awake()
    {
        // Ensure this is the only checkpoint manager in existence
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Set the starting checkpoint once after scene loads
        if (startingCheckpoint)
        {
            SetCheckpoint(startingCheckpoint, initialize: true);
        }
    }

    public bool SetCheckpoint(Checkpoint cp, bool initialize = false)
    {
        if (CurrentCheckpoint == cp && !initialize) return false;

        CurrentCheckpoint = cp;
        return true;
    }

    public Vector3 GetRespawnPosition()
    {
        return CurrentCheckpoint ? CurrentCheckpoint.GetPosition()
                                 : (startingCheckpoint ? startingCheckpoint.GetPosition() : Vector3.zero);
    }

    public Quaternion GetRespawnRotation()
    {
        return CurrentCheckpoint ? CurrentCheckpoint.GetRotation()
                                 : (startingCheckpoint ? startingCheckpoint.GetRotation() : Quaternion.identity);
    }
}
