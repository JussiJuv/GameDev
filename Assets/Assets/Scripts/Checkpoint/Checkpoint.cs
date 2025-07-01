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
    [Tooltip("Local Y-offsset for the prompt")]
    public float promptYOffset = 1.5f;

    private bool isActive;
    private bool playerInRange = false;
    private Collider2D col;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        // On scene load, decide whether this matches the saved checkpoint
        isActive = SaveSystem.Data.lastCheckpointID == checkpointID;
        UpdateVisuals();

        // Init prompt
        if (promptIcon != null)
        {
            promptIcon.gameObject.SetActive(false);
            promptIcon.transform.localPosition = Vector3.up * promptYOffset;
        }
    }

    private void Update()
    {
        // Only allow activation if we are in range, not already active, and press E
        if (playerInRange && !isActive && Input.GetKeyDown(KeyCode.E))
        {
            Activate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActive)
        {
            playerInRange = true;
            if (promptIcon != null)
            {
                promptIcon.gameObject.SetActive(true);
            }
        }
    }

    // Called once when the player presses E in range
    private void Activate()
    {
        isActive = true;
        UpdateVisuals();

        // Hide the prompt and disable this collider
        if (promptIcon != null)
        {
            promptIcon.gameObject.SetActive(false);
        }
        col.enabled = false;

        SaveSystem.SetCheckpoint(checkpointID);

        // TODO: Play SFX, UI

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActive)
        {
            playerInRange = false;
            if (promptIcon != null)
            {
                promptIcon.gameObject.SetActive(false);
            }
        }
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
}
