using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [Header("References")]
    public CurrencyManager currencyManager;
    public TextMeshProUGUI coinText;
    public GameObject popupPrefab;    // assign CoinPopupText prefab here
    public Transform popupParent;     // usually the CoinIcon transform

    private int lastTotal;

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

        if (delta != 0 && popupPrefab != null && popupParent != null)
        {
            // spawn popup under the coin icon
            GameObject go = Instantiate(popupPrefab, popupParent);
            go.transform.localPosition = Vector3.zero;
            var popup = go.GetComponent<CoinPopup>();
            if (popup != null)
                popup.Show(delta);
        }

        lastTotal = newTotal;
    }
}
