using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PortalActivator : MonoBehaviour
{
    [Tooltip("SpriteRenderer for the E icon")]
    public SpriteRenderer promptIcon;
    [Tooltip("Vertical offset for the prompt")]
    public float promptYOffset = 1.5f;

    [Header("Scene & Checkpoint")]
    [Tooltip("Name of the scene to load")]
    public string sceneToLoad = "Hub";
    [Tooltip("Checkpoint ID to set when activating portal")]
    public string nextCheckpointID = "Hub";

    private bool playerInRange = false;

    void Start()
    {
        // Hide prompt at start
        if (promptIcon != null)
        {
            promptIcon.gameObject.SetActive(false);
            promptIcon.transform.localPosition = Vector3.up * promptYOffset;
        }

        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Save the new checkpoint
            SaveSystem.SetCheckpoint(nextCheckpointID);

            SceneManager.LoadScene(sceneToLoad);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && promptIcon != null)
        {
            playerInRange = true;
            promptIcon.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && promptIcon != null)
        {
            playerInRange = false;
            promptIcon.gameObject.SetActive(false);
        }
    }
}
