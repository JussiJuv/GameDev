using UnityEngine;
using System.Collections.Generic;

// Inventory component to hold key items

public class PlayerInventory : MonoBehaviour
{
    // List to store collected keys
    private List<KeyItemData> keys = new List<KeyItemData>();

    public void AddKey(KeyItemData keyData)
    {
        if (keyData == null) return;
        if (!keys.Contains(keyData))
        {
            keys.Add(keyData);
            Debug.Log($"Picked up key: {keyData.keyName}");
            // TODO: Update UI
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

}
