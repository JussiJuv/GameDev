using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    private TextMeshProUGUI coinText;
    private Transform popupParent;
    private CurrencyManager currencyManager;
    private int lastTotal;


    private void Awake()
    {
        // Locate the Coin Container under the Canvas
        var container = GameObject.Find("UI Canvas")?.transform.Find("Coin Container");
        if (container == null) { Debug.LogError("CurrencyUI: 'Coin Container' not found under UI Canvas."); return; }

        // Find the count text and icon parent
        coinText = container.Find("CoinCountText")?.GetComponent<TextMeshProUGUI>();
        popupParent = container.Find("CoinIcon");

        if (coinText == null) Debug.LogError("CurrencyUI: 'CoinCountText' not found.");
        if (popupParent == null) Debug.LogError("CurrencyUI: 'CoinIcon' not found.");

        // Get the CurrencyManager singleton or fallback to finding in scene
        currencyManager = CurrencyManager.Instance;
        if (currencyManager == null)
        {
            currencyManager = FindFirstObjectByType<CurrencyManager>();
            if (currencyManager == null)
            {
                Debug.LogError("[CurrencyUI]: No CurrencyManager found in scene");
            }
        }
    }

    private void OnEnable()
    {
        if (currencyManager != null)
            currencyManager.OnCoinsChanged += OnCoinsChanged;
    }

    private void OnDisable()
    {
        if (currencyManager != null)
            currencyManager.OnCoinsChanged -= OnCoinsChanged;
    }

    private void Start()
    {
        lastTotal = currencyManager != null ? currencyManager.Coins : 0;
        if (coinText != null)
            coinText.text = lastTotal.ToString();
    }

    private void OnCoinsChanged(int newTotal)
    {
        int delta = newTotal - lastTotal;
        if (coinText != null)
            coinText.text = newTotal.ToString();

        if (delta != 0 && popupParent != null)
        {
            // Load popup prefab from Resources (Resources/UI/CoinPopupText)
            var popupPrefab = Resources.Load<GameObject>("UI/CoinPopupText");
            if (popupPrefab != null)
            {
                var go = Instantiate(popupPrefab, popupParent);
                go.transform.localPosition = Vector3.zero;
                var popup = go.GetComponent<CoinPopup>();
                popup?.Show(delta);
            }
        }

        lastTotal = newTotal;
    }
}
