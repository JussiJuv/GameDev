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

    [Header("Prompt Icon")]
    public SpriteRenderer promptIcon;
    public float promptYOffset = 1.5f;

    protected bool isActive;

    private bool playerInRange = false;
    private Collider2D col;

    private void Start()
    {
        col = GetComponent<Collider2D>();

        // Initialize based on saved data
        isActive = (SaveSystem.Data.lastCheckpointID == checkpointID);
        UpdateVisuals();

        if (promptIcon != null)
        {
            promptIcon.gameObject.SetActive(false);
            promptIcon.transform.localPosition = Vector3.up * promptYOffset;
        }
    }

    private void Update()
    {
        if (playerInRange && !isActive && Input.GetKeyDown(KeyCode.E))
            Activate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            playerInRange = true;
            promptIcon?.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            playerInRange = false;
            promptIcon?.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Activates this checkpoint and deactivates all others.
    /// </summary>
    private void Activate()
    {
        // Deactivate every other checkpoint
        var all = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        foreach (var cp in all)
        {
            if (cp == this) continue;
            cp.isActive = false;
            cp.col.enabled = true;
            cp.UpdateVisuals();
        }

        isActive = true;
        UpdateVisuals();

        promptIcon?.gameObject.SetActive(false);
        col.enabled = false;

        // Persist to save data
        SaveSystem.SetCheckpoint(checkpointID);
        // TODO: Play any SFX/UI here
    }

    /// <summary>
    /// Syncs visuals to the isActive flag.
    /// </summary>
    public void UpdateVisuals()
    {
        activeVisual?.SetActive(isActive);
        inactiveVisual?.SetActive(!isActive);
    }
}


