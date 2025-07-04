using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Grid Settings")]
    public int columns = 4;
    public int rows = 3;

    [Header("Potion Icons")]
    public Sprite smallPotionIconSprite;
    public Sprite largePotionIconSprite;

    private GameObject panel;
    private Transform gridContainer;
    private GameObject slotPrefab;
    Transform shopSection;

    private PlayerInventory playerInv;
    private List<SlotUI> slots = new List<SlotUI>();
    private bool isOpen = false;

    void Awake()
    {
        var canvasGO = GameObject.Find("UI Canvas");
        if (canvasGO == null)
        {
            Debug.LogError("[InventoryUI]: 'UI Canvas' not found in scene");
            return;
        }

        var panelTF = canvasGO.transform.Find("InventoryPanel");
        if (panelTF == null)
        {
            Debug.LogError("[InventoryUI]: 'InventoryPanel' not found");
            return;
        }
        panel = panelTF.gameObject;


        gridContainer = panelTF.Find("GridContainer");
        if (gridContainer == null) 
        {
            Debug.LogError("[InventoryUI]: 'GridContainer' not found under panel");
            return;
        }

        // Shop UI
        shopSection = panel?.transform.Find("ShopSection");
        if (shopSection != null)
        {
            shopSection.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("[InventoryUI]: 'ShopSection' not found under panel");
        }

        slotPrefab = Resources.Load<GameObject>("UI/Slot_BG");
        if (slotPrefab == null)
        {
            Debug.LogError("[InventoryUI]: 'InvSlot' prefab not found in Resources");
            return;
        } 

        panel?.SetActive(false);

        CreateSlots();

        playerInv = FindFirstObjectByType<PlayerInventory>();
        if (playerInv == null) Debug.LogError("[InventoryUI]: No PlayerInventory found in scene");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
    }

    private void CreateSlots()
    {
        if (slotPrefab == null || gridContainer == null)
            return;

        // Clear any existing
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        slots.Clear();

        // Create rows x columns slots
        for (int i = 0; i < columns * rows; i++)
        {
            GameObject go = Instantiate(slotPrefab, gridContainer);
            SlotUI slotUI = go.GetComponent<SlotUI>();
            if (slotUI != null)
                slotUI.Clear();
            slots.Add(slotUI);
        }
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;
        panel.SetActive(isOpen);

        // Always keep shop section hidden when toggling with I
        if (shopSection != null) shopSection.gameObject.SetActive(false);

        Time.timeScale = isOpen ? 0f : 1f;

        if (isOpen) RefreshSlots();
    }

    public void RefreshSlots()
    {
        if (playerInv == null)
            return;

        // Clear all slots
        foreach (var slot in slots)
            slot.Clear();

        // Fill slots with player's items in order
        var keys = playerInv.Keys;
        int idx = 0;
        for (; idx < keys.Count && idx < slots.Count; idx++)
        {
            slots[idx].SetItem(keys[idx].keyIcon);
        }

        /*for (int i = 0; i < keys.Count && i < slots.Count; i++)
        {
            var data = keys[i];
            SlotUI slot = slots[i];
            slot.SetItem(data.keyIcon);
        }*/

        // Fill slots with consumables
        var cons = playerInv.Consumables;
        for (int j = 0; j < cons.Count && idx < slots.Count; j++, idx++)
        {
            var s = cons[j];
            Sprite icon = (s.type == ConsumableType.SmallPotion)
                ? smallPotionIconSprite
                : largePotionIconSprite;
            slots[idx].SetItem(icon, s.count);
        }
    }
}