using UnityEngine;

public enum ConsumableType { SmallPotion, LargePotion }

[CreateAssetMenu(menuName = "Data/Consumable Data")]
public class ConsumableData : ScriptableObject
{
    public string displayName;
    public string id;
    public ConsumableType type;
    public Sprite icon;
    [Tooltip("Fraction of max HP to heal (0-1)")]
    [Range(0f, 1f)] public float healFraction;
    public int cost;
    public string description;
}