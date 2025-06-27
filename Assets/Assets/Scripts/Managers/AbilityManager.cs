using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    [Tooltip("List all your AbilityData assets here")]
    public List<AbilityData> allAbilities;

    // Abilities unlocked by name
    private Dictionary<string, AbilityData> _unlocked = new Dictionary<string, AbilityData>();
    // Remaining cooldown time per ability
    private Dictionary<string, float> _cooldowns = new Dictionary<string, float>();

    private void OnEnable()
    {
        if (XPManager.Instance != null)
            XPManager.Instance.OnLevelUp += UnlockAbilities;
    }

    private void OnDisable()
    {
        if (XPManager.Instance != null)
            XPManager.Instance.OnLevelUp -= UnlockAbilities;
    }

    private void Start()
    {
        // Initialize cooldown entries (all ready immediately)
        foreach (var ability in allAbilities)
            _cooldowns[ability.abilityName] = 0f;

        // Unlock abilities for the current level at startup
        int currentLevel = XPManager.Instance != null
            ? XPManager.Instance.currentLevel
            : 1;
        for (int lvl = 1; lvl <= currentLevel; lvl++)
            UnlockAbilities(lvl);
    }

    private void Update()
    {
        // Tick down all active cooldowns
        var keys = new List<string>(_cooldowns.Keys);
        foreach (var name in keys)
        {
            if (_cooldowns[name] > 0f)
                _cooldowns[name] -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Called whenever the player levels up.
    /// Unlocks any abilities whose unlockLevel == level.
    /// </summary>
    private void UnlockAbilities(int level)
    {
        Debug.Log($"[AbilityManager] Checking unlocks for level {level}");
        foreach (var ability in allAbilities)
        {
            if (ability.unlockLevel == level && !_unlocked.ContainsKey(ability.abilityName))
            {
                _unlocked[ability.abilityName] = ability;
                Debug.Log($"[AbilityManager] Unlocked: {ability.abilityName}");
                // TODO: fire an event/UI update about the new ability
            }
        }
    }

    /// <summary>
    /// Returns true if the named ability is unlocked and off cooldown.
    /// </summary>
    public bool CanUse(string abilityName)
    {
        return _unlocked.ContainsKey(abilityName)
               && _cooldowns[abilityName] <= 0f;
    }

    /// <summary>
    /// Grabs the AbilityData for a given abilityName.
    /// </summary>
    public AbilityData GetAbility(string abilityName)
    {
        if (!_unlocked.ContainsKey(abilityName))
        {
            Debug.LogError($"[AbilityManager] GetAbility: '{abilityName}' not unlocked!");
            return null;
        }
        return _unlocked[abilityName];
    }

    /// <summary>
    /// Puts the named ability on its cooldown.
    /// </summary>
    public void Consume(string abilityName)
    {
        if (!_unlocked.ContainsKey(abilityName))
        {
            Debug.LogError($"[AbilityManager] Consume: '{abilityName}' not unlocked!");
            return;
        }
        _cooldowns[abilityName] = _unlocked[abilityName].cooldown;
    }
}
