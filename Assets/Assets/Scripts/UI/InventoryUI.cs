using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();
    }

    void OnEnable()
    {
        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Also handle the current scene immediately
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (playerInv != null)
            playerInv.OnConsumablesChanged -= RefreshSlots;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1) Grab the new Canvas & Panel
        var canvas = GameObject.Find("UI Canvas");
        panel = canvas.transform.Find("InventoryPanel").gameObject;
        gridContainer = panel.transform.Find("GridContainer");
        shopSection = panel.transform.Find("ShopSection");
        if (shopSection != null) shopSection.gameObject.SetActive(false);

        // 2) Ensure slotPrefab is loaded
        if (slotPrefab == null)
            slotPrefab = Resources.Load<GameObject>("UI/Slot_BG");

        // 3) Hide & rebuild
        panel.SetActive(false);
        CreateSlots();


        // (4) Re?find the PlayerInventory in the freshly loaded scene
        playerInv = FindFirstObjectByType<PlayerInventory>();
        if (playerInv == null)
        {
            Debug.LogError("[InventoryUI] No PlayerInventory found in scene " + scene.name);
            return;
        }

        // (5) Subscribe to consumable changes
        playerInv.OnConsumablesChanged -= RefreshSlots;
        playerInv.OnConsumablesChanged += RefreshSlots;

        // (6) Immediately refresh the grid to show keys & potions
        RefreshSlots();
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
            /*if (slotUI != null)
                slotUI.Clear();*/
            if (slotUI == null)
            {
                Debug.LogError("[InventoryUI]: Slot prefab is missing a SlotUI component");
                Destroy(go);
                continue;
            }
            slotUI.Clear();
            slots.Add(slotUI);
        }
    }

    private void ToggleInventory()
    {
        if (panel == null || !panel)
        {
            Debug.LogWarning("[InventoryUI]: Panel is does not exists");
            InitializeUIReferences();
        }

        isOpen = !isOpen;
        panel.SetActive(isOpen);

        // Always keep shop section hidden when toggling with I
        if (shopSection != null) shopSection.gameObject.SetActive(false);

        Time.timeScale = isOpen ? 0f : 1f;

        if (isOpen) RefreshSlots();
    }

    private void InitializeUIReferences()
    {
        var canvas = GameObject.Find("UI Canvas");
        panel = canvas.transform.Find("InventoryPanel").gameObject;
        gridContainer = panel.transform.Find("GridContainer");
        shopSection = panel.transform.Find("ShopSection");
        slotPrefab = Resources.Load<GameObject>("UI/Slot_BG");
        CreateSlots();
        playerInv = FindFirstObjectByType<PlayerInventory>();
        playerInv.OnConsumablesChanged += RefreshSlots;
    }

    public void RefreshSlots()
    {
        /*if (playerInv == null)
            return;*/

        Debug.Log($"[InventoryUI.RefreshSlots] panel={(panel == null ? "NULL" : "OK")}, " +
              $"gridContainer={(gridContainer == null ? "NULL" : "OK")}, " +
              $"slotsCount={slots?.Count}");

        // if UI not yet wired, do it now
        if (panel == null || gridContainer == null || slots.Count == 0)
        {
            Debug.Log("[InventoryUI]: Lazy-init from RefreshSlots");
            InitializeUIReferences();
        }
        if (playerInv == null) FindAnyObjectByType<PlayerInventory>();
        if (playerInv == null) return;

        // Clear all slots
        foreach (var slot in slots)
        {
            //slot.Clear();
            if (slot != null) slot.Clear();
        }

        // Fill slots with player's items in order
        var keys = playerInv.Keys;
        int idx = 0;
        for (; idx < keys.Count && idx < slots.Count; idx++)
        {
            slots[idx].SetItem(keys[idx].keyIcon);
        }

        // Fill slots with consumables
        var cons = playerInv.Consumables;
        for (int j = 0; j < cons.Count && idx < slots.Count; j++, idx++)
        {
            var s = cons[j];
            Sprite icon = (s.type == ConsumableType.SmallPotion)
                ? smallPotionIconSprite
                : largePotionIconSprite;

            var slotUI = slots[idx];
            slotUI.SetItem(icon, s.count);
            //slots[idx].SetItem(icon, s.count);

            var btn = slotUI.GetComponent<Button>();
            if (btn != null)
            {
                var typeToActivate = s.type;

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    Debug.Log($"[InventoryUI] Slot clicked for {typeToActivate}");
                    PlayerInventory.Instance.SetActiveConsumable(typeToActivate);
                    FindFirstObjectByType<ConsumableUI>()?.Refresh();
                });
            }

            /*// Clear old listeners
            slotUI.button.onClick.RemoveAllListeners();
            // If user clicks this spot, activate that potion
            slotUI.button.onClick.AddListener(() =>
            {
                PlayerInventory.Instance.SetActiveConsumable(s.type);
                // Update the potion icon under the hp bar
                FindFirstObjectByType<ConsumableUI>()?.Refresh();

                *//*// Highlight this slot
                HighlightActiveSlot(s.type);*//*
            });*/
        }
    }

    public void DelayedRefresh()
    {
        StartCoroutine(WaitAndRefresh());
    }

    private IEnumerator WaitAndRefresh()
    {
        // Wait for one or two frames to allow inventory to load
        yield return null;
        yield return null;

        playerInv = FindFirstObjectByType<PlayerInventory>();
        if (playerInv != null)
        {
            playerInv.OnConsumablesChanged -= RefreshSlots;
            playerInv.OnConsumablesChanged += RefreshSlots;
            Debug.Log("[InventoryUI] Delayed refresh: found PlayerInventory, refreshing slots");
            RefreshSlots();
        }
        else
        {
            Debug.LogWarning("[InventoryUI] Delayed refresh: still no PlayerInventory found.");
        }
    }
}