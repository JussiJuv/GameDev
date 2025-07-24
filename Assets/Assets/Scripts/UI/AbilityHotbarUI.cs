using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHotbarUI : MonoBehaviour
{
    [Header("References")]
    public AbilityManager abilityManager;   // inspector?set in Demo, but stale after portal
    public Transform slotContainer;
    public GameObject slotPrefab;

    private List<HotbarSlotUI> slots = new List<HotbarSlotUI>();
    private Dictionary<string, HotbarSlotUI> abilityToSlot = new Dictionary<string, HotbarSlotUI>();

    void Awake()
    {
        // Create the 2 slot placeholders once
        for (int i = 0; i < 2; i++)
        {
            var go = Instantiate(slotPrefab, slotContainer);
            slots.Add(go.GetComponent<HotbarSlotUI>());
        }
    }

    private IEnumerator Start()
    {
        // Wait until there's an AbilityManager in the scene hierarchy
        while ((abilityManager = FindFirstObjectByType<AbilityManager>()) == null)
            yield return null;

        abilityManager.OnAbilityUnlocked += OnAbilityUnlocked;
        abilityManager.OnAbilityUsed += OnAbilityUsed;

        RebuildHotbar();
    }

    private void OnDestroy()
    {
        if (abilityManager != null)
        {
            abilityManager.OnAbilityUnlocked -= OnAbilityUnlocked;
            abilityManager.OnAbilityUsed -= OnAbilityUsed;
        }
    }

    private void OnAbilityUnlocked(AbilityData ab)
    {
        foreach (var slot in slots)
        {
            if (!abilityToSlot.ContainsValue(slot))
            {
                slot.Init(ab.icon, CleanKeyLabel(ab.activationKey), Color.gray);
                abilityToSlot[ab.abilityName] = slot;
                break;
            }
        }
    }

    private void OnAbilityUsed(string abilityName, float normalizedCooldown)
    {
        if (abilityToSlot.TryGetValue(abilityName, out var slot))
            slot.SetCooldown(normalizedCooldown);
    }

    public void RebuildHotbar()
    {
        if (abilityManager == null)
        {
            return;
        }

        abilityToSlot.Clear();
        foreach (var slot in slots)
            slot.Clear();

        foreach (var ab in abilityManager.GetUnlockedAbilities())
            OnAbilityUnlocked(ab);
    }

    private string CleanKeyLabel(KeyCode kc)
    {
        if (kc >= KeyCode.Alpha0 && kc <= KeyCode.Alpha9)
            return (kc - KeyCode.Alpha0).ToString();
        return kc.ToString();
    }
}
