using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

            // Load Hub additively
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);

            // Unload demo
            SceneManager.UnloadSceneAsync("demo");

            // Teleport the player to their last checkpoint in the new scene
            StartCoroutine(MovePlayerToSpawnNextFrame());
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only react when Hub loads
        if (scene.name != sceneToLoad) return;

        // Now the Hub is fully initialized—do your checkpoint teleport:
        var lastID = SaveSystem.Data.lastCheckpointID;
        var allCP = Object.FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        var target = System.Array.Find(allCP, cp => cp.checkpointID == lastID);
        if (target != null)
        {
            var player = GameObject.FindWithTag("Player");
            player.transform.position = target.transform.position;

            var camFollow = Object.FindFirstObjectByType<CameraFollow>();
            camFollow?.ForceSnapToPlayer();
        }
        else Debug.LogError($"[PortalActivator] No checkpoint '{lastID}' in scene {scene.name}");

        var playerInv = FindFirstObjectByType<PlayerInventory>();
        playerInv?.DebugLogInventory();


        // Apply saved state manually
        var applier = FindFirstObjectByType<SaveStateApplier>();
        if (applier != null)
        {
            Debug.Log("[PortalActivator] Found SaveStateApplier. Forcing ApplySavedState().");
            applier.ApplySavedState();
        }
        else
        {
            Debug.LogWarning("[PortalActivator] SaveStateApplier not found in Hub scene!");
        }

        var ui = FindFirstObjectByType<InventoryUI>();
        if (ui != null)
        {
            Debug.Log("[PortalActivator] Found InventoryUI, calling DelayedRefresh()");
            ui.DelayedRefresh();
        }
        else
        {
            Debug.LogWarning("[PortalActivator] InventoryUI not found in UI scene");
        }
    }


    private IEnumerator MovePlayerToSpawnNextFrame()
    {
        // Wait one frame so all Hub scene objects (including Checkpoints) exist
        yield return null;

        // 1) Which checkpoint ID were we at?
        string lastID = SaveSystem.Data.lastCheckpointID;
        if (string.IsNullOrEmpty(lastID))
        {
            Debug.LogWarning("[PortalActivator] No saved checkpoint ID");
            yield break;
        }

        // 2) Find the matching Checkpoint component
        Checkpoint targetCP = null;
        foreach (var cp in FindObjectsByType<Checkpoint>(FindObjectsSortMode.None))
        {
            if (cp.checkpointID == lastID)
            {
                targetCP = cp;
                break;
            }
        }
        if (targetCP == null)
        {
            Debug.LogError($"[PortalActivator] Checkpoint '{lastID}' not found in Hub scene");
            yield break;
        }

        // 3) Move the Player there
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[PortalActivator] Player not found to move!");
            yield break;
        }
        player.transform.position = targetCP.transform.position;

        // 4) Snap the camera if you have a CameraFollow
        var camFollow = FindFirstObjectByType<CameraFollow>();
        if (camFollow != null)
            camFollow.ForceSnapToPlayer();
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

    private IEnumerator DelayedUIRefresh()
    {
        // Wait one frame
        yield return null;

        var invUI = FindFirstObjectByType<InventoryUI>();
        if (invUI != null)
        {
            Debug.Log("[PortalActivator]: Manually refreshing Inventory UI");
            invUI.RefreshSlots();
        }
        else
        {
            Debug.LogWarning("[PortalActivator] Could not find InventoryUI to refresh");
        }
    }
}
