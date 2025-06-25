using UnityEngine;

// ScriptableObject representing a unique key that opens a specific door.

[CreateAssetMenu(fileName = "New KeyItem", menuName = "Inventory/Key Item")]
public class KeyItemData : ScriptableObject
{
    [Header("Key Properties")]
    public string keyName;
    public Sprite keyIcon;
    public string doorID;
}