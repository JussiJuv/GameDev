using UnityEngine;

public enum AbilityType { Active, Passive }

[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/AbilityData")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public AbilityType type;
    public int unlockLevel;

    [Header("Active Settings")]
    public KeyCode activationKey;
    public float cooldown;

    [Header("Dash Settings (if Active)")]
    public float dashDistance;
    public float dashSpeed;

    [Header("Arrow Rain Settings (if Active)")]
    public int arrowCount;
    public float areaRadius;
    public int arrowDamage;
    public GameObject arrowPrefab;
    public float dropHeight;

}
