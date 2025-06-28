using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    [Header("References")]
    public XPManager xpManager;
    public Image fillImage;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI fractionText;

    private void OnEnable()
    {
        if (xpManager != null)
        {
            xpManager.OnXPChanged += UpdateBar;
            xpManager.OnLevelUp += OnLevelUp;
        }
    }

    private void OnDisable()
    {
        if (xpManager != null)
        {
            xpManager.OnXPChanged -= UpdateBar;
            xpManager.OnLevelUp -= OnLevelUp;
        }
    }

    private void Start()
    {
        if (xpManager != null)
        {
            UpdateBar(xpManager.currentXP, xpManager.xpToNextLevel);
            levelText.text = xpManager.currentLevel.ToString();
        }
    }

    private void UpdateBar(int currentXP, int xpToNext)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentXP / (float)xpToNext;
        }

        if (fractionText != null)
        {
            fractionText.text = $"{currentXP} / {xpToNext}";
        }
    }

    private void OnLevelUp(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = newLevel.ToString();
        }
    }
}
