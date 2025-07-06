using UnityEngine;

/// <summary>
/// On scene start, moves the player to the last saved checkpoint position.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    [Header("SKIP SAVE MECHANIC, DEVELOPER MODE")]
    [Tooltip("When checked, do not teleport to saved checkpoint on Start")]
    public bool skipCheckpointOnStart = false;

    void Start()
    {
        if (skipCheckpointOnStart) return;

        // Find player by tag
        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO == null)
        {
            Debug.LogError("[CheckpointManager]: No GameObject tagged 'Player' in scene");
            return;
        }

        var playerTransform = playerGO.transform;

        // Get saved checkpoint ID
        string savedID = SaveSystem.Data.lastCheckpointID;
        if (string.IsNullOrEmpty(savedID)) return;

        // Find all checkpoints in this scene
        var allCPs = Object.FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        foreach (var cp in allCPs)
        {
            if (cp.checkpointID == savedID)
            {
                playerTransform.position = cp.transform.position;
                Debug.Log($"[CheckpointManager]: Warped player to '{savedID}' at {cp.transform.position}");
                break;
            }
        }
    }
}
