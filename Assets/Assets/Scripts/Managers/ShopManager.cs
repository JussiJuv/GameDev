using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public string id;
        public string name;
        [TextArea] public string description;
        public int cost;
        public Sprite icon;
    }

    [Header("Data")]
    public List<ShopItem> items;

    [Header("UI Paths (under ShopSection)")]
    public string itemListPath = "ItemList/Viewport/Content";
    public string detailsPanelName = "DetailsPanel";
    public string entryPrefabName = "ShopItemEntry";

    private Transform itemListContent;
    private GameObject entryPrefab;

    private TextMeshProUGUI nameText, descText, costText;
    private Button buyButton, backButton;
    private Image costIcon;

    private GameObject shopUI;
    private ShopItem selected;

    void Awake()
    {
        shopUI = GameObject.Find("InventoryPanel");

        // Bind list container
        var listTf = transform.Find(itemListPath);
        if (listTf == null)
            Debug.LogError($"ShopManager: '{itemListPath}' not found under {name}.");
        else
            itemListContent = listTf;

        // Load prefab
        entryPrefab = Resources.Load<GameObject>($"UI/{entryPrefabName}");
        if (entryPrefab == null)
            Debug.LogError($"ShopManager: Prefab 'Resources/UI/{entryPrefabName}.prefab' not found.");

        // Bind DetailsPanel
        var details = transform.Find(detailsPanelName);
        if (details == null)
        {
            Debug.LogError($"ShopManager: '{detailsPanelName}' not found under {name}.");
            return;
        }

        nameText = details.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        descText = details.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
        costText = details.Find("CostContainer/CostText")?.GetComponent<TextMeshProUGUI>();
        costIcon = details.Find("CostContainer/CostIcon")?.GetComponent<Image>();
        buyButton = details.Find("BuyButton")?.GetComponent<Button>();
        backButton = details.Find("BackButton")?.GetComponent<Button>();

        // Validate
        if (nameText == null) Debug.LogError("ShopManager: 'NameText' missing!");
        if (descText == null) Debug.LogError("ShopManager: 'DescriptionText' missing!");
        if (costText == null) Debug.LogError("ShopManager: 'CostText' missing under CostContainer!");
        if (costIcon == null) Debug.LogError("ShopManager: 'CostIcon' missing under CostContainer!");
        if (buyButton == null) Debug.LogError("ShopManager: 'BuyButton' missing!");
        if (backButton == null) Debug.LogError("ShopManager: 'BackButton' missing!");

        buyButton?.onClick.AddListener(Buy);
        backButton?.onClick.AddListener(Close);

        // Clear
        if (nameText != null) nameText.text = "";
        if (descText != null) descText.text = "";
        if (costText != null) costText.text = "";
        if (costIcon != null) costIcon.gameObject.SetActive(false);
    }

    void Start()
    {
        if (itemListContent == null || entryPrefab == null) return;

        foreach (var it in items)
        {
            var go = Instantiate(entryPrefab, itemListContent);

            // Icon
            var iconImg = go.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImg != null) iconImg.sprite = it.icon;

            // Label
            var label = go.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = it.name;

            // Correct closure
            var captured = it;
            var btn = go.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => Select(captured));
        }
    }

    void Select(ShopItem it)
    {
        selected = it;
        Debug.Log($"ShopManager: Selected {it.name}");

        if (nameText != null) nameText.text = it.name;
        if (descText != null) descText.text = it.description;
        if (costText != null) costText.text = $"{it.cost}";

        if (costIcon != null)
            costIcon.gameObject.SetActive(true);
    }

    void Buy()
    {
        if (selected == null)
        {
            Debug.LogError("ShopManager.Buy: no item selected!");
            return;
        }

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("ShopManager.Buy: CurrencyManager.Instance is null!");
            return;
        }

        bool couldSpend = CurrencyManager.Instance.SpendCoins(selected.cost);
        Debug.Log($"ShopManager.Buy: SpendCoins returned {couldSpend}");

        if (!couldSpend)
        {
            Debug.Log("ShopManager: Not enough coins");
            return;
        }

        // at this point we have successfully spent coins
        Debug.Log($"ShopManager: Purchased {selected.name}");

        // Try to get the inventory
        PlayerInventory inv = PlayerInventory.Instance;
        if (inv == null)
        {
            // fallback: look it up in scene
            inv = FindFirstObjectByType<PlayerInventory>();
            if (inv == null)
            {
                Debug.LogError("ShopManager.Buy: No PlayerInventory instance found!");
                return;
            }
            else Debug.LogWarning("ShopManager.Buy: Using FindObjectOfType fallback for PlayerInventory.");
        }

        // Map the purchase
        switch (selected.id)
        {
            case "small_pot":
                inv.AddConsumable(ConsumableType.SmallPotion, 1);
                break;
            case "large_pot":
                inv.AddConsumable(ConsumableType.LargePotion, 1);
                break;
            default:
                Debug.LogWarning($"[ShopManager] unrecognized item id {selected.id}");
                break;
        }

        // Finally refresh the UI
        var ui = FindFirstObjectByType<InventoryUI>();
        if (ui != null) ui.RefreshSlots();
    }



    /*void Buy()
    {
        if (selected == null)
        {
            Debug.LogError("[ShopManager]: no item selected");
            return;
        }

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[ShopManager]: CurrencyManager.Instance is null");
            return;
        }

        if (CurrencyManager.Instance.SpendCoins(selected.cost))
        {
            Debug.Log($"Purchased {selected.name}");
            if (selected.id == "small_pot")
            {
                PlayerInventory.Instance.AddConsumable(ConsumableType.SmallPotion);
            }
            else if (selected.id == "large_pot")
            {
                PlayerInventory.Instance.AddConsumable(ConsumableType.LargePotion);
            }
        }
        else
            Debug.Log("Not enough coins");
    }*/

    void Close()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
