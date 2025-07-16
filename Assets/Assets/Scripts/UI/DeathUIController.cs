using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathUIController : MonoBehaviour
{
    public GameObject panel;
    public Button respawnButton;
    public Button quitButton;

    private Health playerHealth;
    private GameObject playerGO;
    private Transform playerTransform;

    private static string pendingCheckpointID;

    private void Awake()
    {
        panel.SetActive(false);
        StartCoroutine(FindAndSubscribe());
    }

    private IEnumerator FindAndSubscribe()
    {
        while ((playerGO = GameObject.FindWithTag("Player")) == null)
            yield return null;

        playerTransform = playerGO.transform;
        playerHealth = playerGO.GetComponent<Health>();
        if (playerHealth == null)
        {
            Debug.LogError("[DeathUIController]: Player exists but has no Health component");
        }
        else
        {
            playerHealth.OnDeath.AddListener(OnPlayerDied);
            Debug.Log("[DeathUIController]: Subscribed to Player Health.OnDeath");
        }

    }

    private void OnPlayerDied()
    {
        Time.timeScale = 0f;
        panel.SetActive(true);

        respawnButton.onClick.RemoveAllListeners();
        respawnButton.onClick.AddListener(Respawn);

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(QuitToMainMenu);
    }
   

    private void Respawn()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);

        pendingCheckpointID = SaveSystem.Data.lastCheckpointID;

        // Reload the scene
        string scene = SaveSystem.Data.lastScene;
        if (string.IsNullOrEmpty(scene))
        {
            Debug.LogError("[DeathUIController]: No lastScene recorded in save data");
            return;
        }

        /*SceneManager.sceneLoaded += StaticHandleRespawnAfterLoad;
        SceneManager.LoadScene(scene);*/

        /*SceneManager.sceneLoaded += OnSceneLoadedAfterRespawn;

        SceneManager.UnloadSceneAsync(scene);
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);*/

        StartCoroutine(ReloadScene(scene));
    }

    private IEnumerator ReloadScene(string sceneName)
    {
        /*// Unload it fully
        var unload = SceneManager.UnloadSceneAsync(sceneName);
        if (unload == null)
        {
            Debug.LogError("[DeathUIController]: Failed to start unloading");
            yield break;
        }
        yield return unload;*/

        Scene toUnload = SceneManager.GetSceneByName(sceneName);
        if (!toUnload.isLoaded)
            Debug.LogError($"[DeathUIController]: Scene '{sceneName}' not currently loaded, cannot unload");
        else
        {
            var unloadOp = SceneManager.UnloadSceneAsync(toUnload);
            yield return unloadOp;
        }

        // Load it back additively
        var load = SceneManager.LoadSceneAsync(sceneName,  LoadSceneMode.Additive);
        yield return load;

        yield return HandleRespawnAfterLoad();
    }

    private void OnSceneLoadedAfterRespawn(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != SaveSystem.Data.lastScene) return;

        SceneManager.sceneLoaded -= OnSceneLoadedAfterRespawn;
        StartCoroutine(HandleRespawnAfterLoad());
    }

    private IEnumerator HandleRespawnAfterLoad()
    {
        yield return null;

        // Find fresh player
        playerGO = GameObject.FindWithTag("Player");
        if (playerGO == null)
        {
            Debug.LogError("[DeathUIController]: Player not found after reload");
            yield break;
        }

        playerTransform = playerGO.transform;
        playerHealth = playerGO.GetComponent<Health>();
        if (playerHealth == null)
        {
            Debug.LogError("[DeathUIController]: Player has no Health component after reload");
            yield break;
        }

        playerHealth.Revive();

        // Teleport to pending checkpoint
        var allCP = Object.FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        var target = System.Array.Find(allCP, cp => cp.checkpointID == pendingCheckpointID);
        if (target != null)
        {
            playerTransform.position = target.transform.position;
        }
        else
        {
            Debug.LogError($"[DeathUIController]: Checkpoint '{pendingCheckpointID} not found'");
        }

        // Restore saved stats/inventory
        FindFirstObjectByType<SaveStateApplier>()?.ApplySavedState();

        // Snap camera
        var cam = FindFirstObjectByType<CameraFollow>();
        StartCoroutine(DelayedSnap(cam));
    }
    
    private void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        Debug.Log("Quitting...");
        //SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator DelayedSnap(CameraFollow cam)
    {
        // Wait one frame so all transforms have updated
        yield return null;
        cam.initialized = false;
        cam.ForceSnapToPlayer();
    }

    public static class RespawnData
    {
        public static string PendingCheckpointID;
    }

}
