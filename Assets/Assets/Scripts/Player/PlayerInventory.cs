using UnityEngine;
using System.Collections.Generic;

// Inventory component to hold key items

public enum ConsumableType { SmallPotion, LargePotion }

[System.Serializable]
public struct InventorySlot
{
    public ConsumableType type;
    public int count;
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    // List to store collected keys
    private List<KeyItemData> keys = new List<KeyItemData>();

    private List<InventorySlot> consumableSlots = new List<InventorySlot>();

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

    /// <summary>
    /// Expose the list of collected keys for UI only.
    /// </summary>
    public IReadOnlyList<KeyItemData> Keys => keys;

    public void ClearKeys() => keys.Clear();

    /*public void AddConsumable(ConsumableType type, int amt = 1)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].type == type)
            {
                slots[i] = new InventorySlot { type = type, count = slots[i].count + amt };
                return;
            }
        }
        // First time
        slots.Add(new InventorySlot { type = type, count = amt });
    }*/
    public void AddConsumable(ConsumableType type, int amount = 1)
    {
        for (int i = 0; i < consumableSlots.Count; i++)
        {
            if (consumableSlots[i].type == type)
            {
                var slot = consumableSlots[i];
                slot.count += amount;
                consumableSlots[i] = slot;
                Debug.Log($"Added {amount} to {type}, new count = {slot.count}");
                return;
            }
        }
        // first time purchase of this type
        consumableSlots.Add(new InventorySlot { type = type, count = amount });
        Debug.Log($"Created new slot {type} = {amount}");
    }

    public IReadOnlyList<InventorySlot> Consumables => consumableSlots;

}
