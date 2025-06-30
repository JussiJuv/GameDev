using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The boss's Health component")]
    public Health bossHealth;

    [Tooltip("Image component of the fill graphic")]
    public Image fillImage;

    [Tooltip("TextMeshPro label showing boss name")]
    public TMP_Text bossNameText;

    private void Update()
    {
        if (bossHealth == null || fillImage == null)
            return;

        // Update fill amount
        float normalized = bossHealth.currentHP / (float)bossHealth.maxHP;
        fillImage.fillAmount = normalized;
    }

    /// <summary>
    /// Call to show and initialize the bar
    /// </summary>
    public void Show(Health health, string bossName)
    {
        bossHealth = health;
        bossNameText.text = bossName;
        fillImage.fillAmount = 1f;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Call to hide the bar
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
