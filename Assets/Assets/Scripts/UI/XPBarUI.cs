using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    private Image fillImage;
    private TextMeshProUGUI levelText;
    private TextMeshProUGUI fractionText;

    private XPManager xpManager;

    private void Awake()
    {
        // Locate the HUD parent under the Canvas
        var hud = GameObject.Find("UI Canvas")?.transform.Find("HUD");
        if (hud == null) { Debug.LogError("XPBarUI: HUD not found under UI Canvas."); return; }

        // Find the XP_BG container
        var xpBg = hud.Find("XP_BG");
        if (xpBg == null) { Debug.LogError("XPBarUI: XP_BG not found under HUD."); return; }

        // Grab the fill image and text fields
        fillImage = xpBg.Find("XP_Fill")?.GetComponent<Image>();
        levelText = xpBg.Find("XP_LevelText")?.GetComponent<TextMeshProUGUI>();
        fractionText = xpBg.Find("XP_FractionText")?.GetComponent<TextMeshProUGUI>();

        if (fillImage == null) Debug.LogError("XPBarUI: XP_Fill Image not found.");
        if (levelText == null) Debug.LogError("XPBarUI: XP_LevelText not found.");
        if (fractionText == null) Debug.LogError("XPBarUI: XP_FractionText not found.");

        // Get the XPManager singleton
        xpManager = XPManager.Instance;
        if (xpManager == null) Debug.LogError("XPBarUI: No XPManager instance present.");
    }

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

    public void UpdateBar(int currentXP, int xpToNext)
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

    public void OnLevelUp(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = newLevel.ToString();
        }
    }
}
