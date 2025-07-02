using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    /*[Header("UI References")]
    public GameObject panel;               // InventoryPanel
    public Transform gridContainer;        // GridContainer transform
    public GameObject slotPrefab;          // InvSlot prefab*/

    [Header("Grid Settings")]
    public int columns = 5;
    public int rows = 5;

    private GameObject panel;
    private Transform gridContainer;
    private GameObject slotPrefab;
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

        /*playerInv = FindFirstObjectByType<PlayerInventory>();
        if (panel != null)
            panel.SetActive(false);

        // Instantiate grid slots
        CreateSlots();*/
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
        Time.timeScale = isOpen ? 0f : 1f;

        if (isOpen)
            RefreshSlots();
    }

    private void RefreshSlots()
    {
        if (playerInv == null)
            return;

        // Clear all slots
        foreach (var slot in slots)
            slot.Clear();

        // Fill slots with player's items in order
        var keys = playerInv.Keys;
        for (int i = 0; i < keys.Count && i < slots.Count; i++)
        {
            var data = keys[i];
            SlotUI slot = slots[i];
            slot.SetItem(data.keyIcon);
        }
    }
}