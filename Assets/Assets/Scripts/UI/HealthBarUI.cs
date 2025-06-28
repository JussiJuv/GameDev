using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    public Health playerHealth;
    public Image fillImage;

    private void Update()
    {
        if (playerHealth == null || fillImage == null)
        {
            return; 
        }

        float normalized = playerHealth.currentHP / (float)playerHealth.maxHP;
        fillImage.fillAmount = normalized;
    }
}
