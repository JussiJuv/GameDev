using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaResetter : MonoBehaviour
{
    [Header("References")]
    public WaveArenaController waveController;
    public Transform enemyContainer;
    public StoneGateController gateController;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == SaveSystem.Data.lastScene)
            ResetArena();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == SaveSystem.Data.lastScene)
            ResetArena();
    }

    void ResetArena()
    {
        waveController.ResetWaves();

        waveController.ResetArenaContents(enemyContainer);

        // Destroy any hanging imp projectiles
        foreach (var proj in FindObjectsByType<ImpProjectile>(FindObjectsSortMode.None))
            Destroy(proj.gameObject);

    }
}
