using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

//public enum ConsumableType { SmallPotion, LargePotion }

[System.Serializable]
public struct InventorySlot
{
    public ConsumableType type;
    public int count;
}


public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    // Events
    public event Action OnConsumablesChanged;
    public event Action<ConsumableType?> OnActiveConsumableChanged;

    // Drag in both ConsumableData assets here
    [SerializeField] public List<ConsumableData> consumableDataList;

    // Tracks counts
    private List<InventorySlot> consumableSlots = new List<InventorySlot>();

    // Currently selected type
    private ConsumableType? activeConsumable;
    public ConsumableType? ActiveConsumable => activeConsumable;

    // List to store collected keys
    private List<KeyItemData> keys = new List<KeyItemData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Default to first available type if any
        if (consumableSlots.Any())
        {
            activeConsumable = consumableSlots[0].type;
            OnActiveConsumableChanged?.Invoke(activeConsumable);
        }
    }

    public void ClearConsumables()
    {
        consumableSlots.Clear();
        activeConsumable = null;
        OnConsumablesChanged?.Invoke();
        OnActiveConsumableChanged?.Invoke(activeConsumable);
    }

    /// <summary>
    /// Force a specific consumable type as active
    /// </summary>
    public void SetActiveConsumable(ConsumableType? type)
    {
        activeConsumable = type;
        OnActiveConsumableChanged?.Invoke(activeConsumable);
    }

    public void AddConsumable(ConsumableType type, int amount = 1)
    {
        var index = consumableSlots.FindIndex(s => s.type == type);
        if (index >= 0)
        {
            var slot = consumableSlots[index];
            slot.count += amount;
            consumableSlots[index] = slot;
        }
        else
        {
            consumableSlots.Add(new InventorySlot { type = type, count = amount });
        }

        // If no active yet, set this
        if (!activeConsumable.HasValue)
        {
            activeConsumable = type;
            OnActiveConsumableChanged?.Invoke(activeConsumable);
        }
        OnConsumablesChanged?.Invoke();
    }

    public void AddKey(KeyItemData keyData)
    {
        if (keyData == null) return;
        if (!keys.Contains(keyData))
        {
            keys.Add(keyData);
            Debug.Log($"Picked up key: {keyData.keyName}");
        }
    }

    public bool HasKey(string doorID)
    {
        return keys.Exists(k => k.doorID == doorID);
    }

    public bool UseKey(string doorID)
    {
        KeyItemData key = keys.Find(k => k.doorID == doorID);
        if (key != null)
        {
            Debug.Log($"Used key for door: {doorID}");
            return true;
        }
        return false;
    }

    public bool ConsumeActive(out float healFraction)
    {
        healFraction = 0f;
        if (!activeConsumable.HasValue) return false;

        int idx = consumableSlots.FindIndex(s => s.type == activeConsumable.Value);
        if (idx < 0 || consumableSlots[idx].count <= 0)
        {
            // Auto-switch
            var other = consumableSlots.FirstOrDefault(s => s.count > 0);
            activeConsumable = other.count > 0 ? (ConsumableType?)other.type : null;
            OnActiveConsumableChanged?.Invoke(activeConsumable);
            return false;
        }

        // Decrement slot count
        var slot = consumableSlots[idx];
        slot.count--;
        consumableSlots[idx] = slot;
        OnConsumablesChanged?.Invoke();

        // Output fraction from data
        var data = consumableDataList.First(d => d.type == slot.type);
        healFraction = data.healFraction;

        // Auto-switch if depleted
        if (slot.count == 0)
        {
            var other = consumableSlots.FirstOrDefault(s => s.count > 0);
            activeConsumable = other.count > 0 ? (ConsumableType?)other.type : null;
            OnActiveConsumableChanged?.Invoke(activeConsumable);
        }

        return true;
    }


    /// <summary>
    /// Expose the list of collected keys for UI only.
    /// </summary>
    public IReadOnlyList<KeyItemData> Keys => keys;

    public void ClearKeys() => keys.Clear();

    public IReadOnlyList<InventorySlot> Consumables => consumableSlots;

}
