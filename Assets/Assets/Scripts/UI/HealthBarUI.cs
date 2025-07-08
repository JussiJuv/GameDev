using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBarUI : MonoBehaviour
{
    [Header("References (will be found at runtime)")]
    [Tooltip("The player's Health component")]
    public Health playerHealth;

    [Tooltip("Image component of the fill graphic")]
    public Image fillImage;

    private void Awake()
    {
        if (playerHealth == null)
            TryFindPlayerHealth();

        // Listen for future scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // After any new scene comes online, re?resolve the playerHealth reference
        TryFindPlayerHealth();
    }

    private void TryFindPlayerHealth()
    {
        // 1) look for an object tagged "Player"
        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
        {
            var h = playerGO.GetComponent<Health>();
            if (h != null)
            {
                playerHealth = h;
                return;
            }
        }

        // 2) fallback: just grab the first Health in the scene
        playerHealth = Object.FindFirstObjectByType<Health>();

        if (playerHealth == null)
            Debug.LogError("[HealthBarUI] Couldn't find a Health component for the player!");
    }

    private void Update()
    {
        // If we still don’t have one, bail out
        if (playerHealth == null || fillImage == null)
            return;

        // Update the bar
        float normalized = playerHealth.currentHP / (float)playerHealth.maxHP;
        fillImage.fillAmount = normalized;
    }
}
