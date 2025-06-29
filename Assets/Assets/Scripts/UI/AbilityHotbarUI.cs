using UnityEngine;
using System.Collections.Generic;

public class AbilityHotbarUI : MonoBehaviour
{
    [Header("References")]
    public AbilityManager abilityManager;
    public Transform slotContainer;
    public GameObject slotPrefab;

    private List<HotbarSlotUI> slots = new List<HotbarSlotUI>();
    private Dictionary<string, HotbarSlotUI> abilityToSlot = new Dictionary<string, HotbarSlotUI>();

    private void Start()
    {
        // Preallocate exactly 3 slots
        for (int i= 0; i < 3; i++)
        {
            var go = Instantiate(slotPrefab, slotContainer);
            var ui = go.GetComponent<HotbarSlotUI>();
            slots.Add(ui);
        }

        // Listen for unlocks
        abilityManager.OnAbilityUnlocked += OnAbilityUnlocked;
        abilityManager.OnAbilityUsed += OnAbilityUsed;
    }

    private void OnAbilityUnlocked(AbilityData ab)
    {
        // find first empty slot
        foreach (var slot in slots)
        {
            if (!abilityToSlot.ContainsValue(slot))
            {
                //slot.Init(ab.icon, ab.activationKey.ToString(), Color.gray);
                slot.Init(ab.icon, CleanKeyLabel(ab.activationKey), Color.gray);
                abilityToSlot[ab.abilityName] = slot;
                break;
            }
        }
    }

    private void OnAbilityUsed(string abilityName, float normalizedCooldown)
    {
        if (abilityToSlot.TryGetValue(abilityName, out var slot))
        {
            slot.SetCooldown(normalizedCooldown);
        }
    }

    // Converts KeyCode.Alpha1 -> "1"
    private string CleanKeyLabel(KeyCode kc)
    {
        // Numeric keys (Alpha0-9) -> strip "Alpha"
        if (kc >= KeyCode.Alpha0 && kc <= KeyCode.Alpha9)
        {
            return (kc - KeyCode.Alpha0).ToString();
        }

        // Otherwise just use the enum name
        return kc.ToString();   
    }
}
