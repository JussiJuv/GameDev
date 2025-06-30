using UnityEngine;

/// <summary>
/// On scene start, moves the player to the last saved checkpoint position.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    [Header("SKIP SAVE MECHANIC, DEVELOPER MODE")]
    [Tooltip("When checked, do not teleport to saved checkpoint on Start")]
    public bool skipCheckpointOnStart = false;

    [Tooltip("Drag your Player GameObject here (or its Transform).")]
    public Transform playerTransform;

    void Start()
    {
        if (skipCheckpointOnStart) return;

        // Find the saved checkpoint in scene
        string savedID = SaveSystem.Data.lastCheckpointID;
        if (string.IsNullOrEmpty(savedID))
            return;

        var allCheckpoints = Object.FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);

        foreach (var cp in allCheckpoints)
        {
            if (cp.checkpointID == savedID)
            {
                playerTransform.position = cp.transform.position;
                break;
            }
        }
    }
}
