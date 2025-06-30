using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Unique ID for this checkpoint")]
    public string checkpointID;

    [Tooltip("The prefab variant shown when active")]
    public GameObject activeVisual;

    [Tooltip("The prefab variant shown when inactive")]
    public GameObject inactiveVisual;

    private bool isActive;

    private void Start()
    {
        // On scene load, decide whether this matches the saved checkpoint
        isActive = SaveSystem.Data.lastCheckpointID == checkpointID;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        activeVisual.SetActive(isActive); 
        inactiveVisual.SetActive(!isActive);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isActive) return;
        if (!collision.CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.E)) Activate();
    }

    /// <summary>
    /// Switch this checkpoint to active, persist, and give feedback
    /// </summary>
    private void Activate()
    {
        isActive = true;
        UpdateVisuals();
        SaveSystem.SetCheckpoint(checkpointID);

        // TODO: Play SFX, UI
    }
}
