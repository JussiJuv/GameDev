using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBarUI : MonoBehaviour
{
    private Image fillImage;
    private TextMeshProUGUI levelText;

    private XPManager xpManager;

    private void Awake()
    {
        fillImage = transform.Find("XP_Fill")?.GetComponent<Image>();
        if (fillImage == null)
            Debug.LogError("[XPBarUI]: XP_Fill Image not found under XP_BG");

        var canvas = GameObject.Find("UI Canvas")?.transform;
        var levelTextTransform = canvas?.Find("HUD/HUD_Panel/LevelDiamond/LevelText");
        if (levelTextTransform != null)
            levelText = levelTextTransform.GetComponent<TextMeshProUGUI>();
        if (levelText == null)
            Debug.LogError("[XPBarUI]: LevelText TMP not found");

        xpManager = XPManager.Instance;
        if (xpManager == null)
            Debug.LogError("[XPBarUI]: No XPManager instance present");
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
    }

    public void OnLevelUp(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = newLevel.ToString();
        }
    }
}
