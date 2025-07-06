using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBootstrap : MonoBehaviour
{
    [Tooltip("Player prefab to instantiate if not carried over")]
    public GameObject playerPrefab;

    void Awake()
    {
        // Do we already have a player in memory?
        if (FindFirstObjectByType<PlayerInventory>() != null)
        {
            Debug.Log("[SceneBootstrap] Player already present, skipping spawn.");
            return;
        }

        // No player exists (e.g. direct scene play), spawn one
        Debug.Log("[SceneBootstrap] Spawning new Player from prefab.");
        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
    }
}
