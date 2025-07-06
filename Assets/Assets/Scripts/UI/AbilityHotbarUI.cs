using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AbilityHotbarUI : MonoBehaviour
{
    [Header("References")]
    public AbilityManager abilityManager;   // inspector?set in Demo, but stale after portal
    public Transform slotContainer;
    public GameObject slotPrefab;

    private List<HotbarSlotUI> slots = new List<HotbarSlotUI>();
    private Dictionary<string, HotbarSlotUI> abilityToSlot = new Dictionary<string, HotbarSlotUI>();

    private bool subscribed = false;

    private void Start()
    {
        if (abilityManager == null)
        {
            abilityManager = FindFirstObjectByType<AbilityManager>();
        }

        if (!subscribed && abilityManager != null)
        {
            abilityManager.OnAbilityUnlocked += OnAbilityUnlocked;
            abilityManager.OnAbilityUsed += OnAbilityUsed;
            subscribed = true;
        }

        RebuildHotbar();
    }

    void Awake()
    {
        // Create the 3 slot placeholders once
        for (int i = 0; i < 3; i++)
        {
            var go = Instantiate(slotPrefab, slotContainer);
            slots.Add(go.GetComponent<HotbarSlotUI>());
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // If portal into Hub happened without destroying this UI,
        // manually invoke our scene?loaded handler now
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (abilityManager != null)
        {
            abilityManager.OnAbilityUnlocked -= OnAbilityUnlocked;
            abilityManager.OnAbilityUsed -= OnAbilityUsed;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var mgr = FindFirstObjectByType<AbilityManager>();
        if (mgr != abilityManager)
        {
            // Unsubscribe old
            if (subscribed && abilityManager != null)
            {
                abilityManager.OnAbilityUnlocked -= OnAbilityUnlocked;
                abilityManager.OnAbilityUsed -= OnAbilityUsed;
                subscribed = false;
            }

            abilityManager = mgr;

            // Subscribe once
            if (!subscribed && abilityManager != null)
            {
                abilityManager.OnAbilityUnlocked += OnAbilityUnlocked;
                abilityManager.OnAbilityUsed += OnAbilityUsed;
                subscribed = true;
            }
        }

        RebuildHotbar();
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
