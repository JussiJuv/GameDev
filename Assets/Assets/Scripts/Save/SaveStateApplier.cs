using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveStateApplier : MonoBehaviour
{
    private void Awake()
    {
        SaveSystem.Load();
        ApplySavedState();

        // Listen for any scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Clean up the callback to avoid leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        Debug.Log($"[SaveStateApllier] Scene loaded: {scene.name}");
        // Whenever a new scene comes up, reapply the saved data
        ApplySavedState();

        // Debugs for save file
        // Debug after applying
        if (XPManager.Instance != null && CurrencyManager.Instance != null)
        {
            Debug.Log($"[SaveStateApplier] After load ? Level={XPManager.Instance.currentLevel}, XP={XPManager.Instance.currentXP}, Coins={CurrencyManager.Instance.Coins}");
        }

        // Refresh Inventory UI
        FindFirstObjectByType<InventoryUI>()?.RefreshSlots();

        // Re-synnc abilities
        var am = FindFirstObjectByType<AbilityManager>();
        if (am != null)
        {
            am.InitializeUnlockedAbilities(SaveSystem.Data.savedLevel);
        }

        // Force the ability hotbar UI to rebuild
        FindFirstObjectByType<AbilityHotbarUI>()?.RebuildHotbar();
    }

    private void ApplySavedState()
    {
        // Health
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var hp = player.GetComponent<Health>();
            if (hp != null && SaveSystem.Data.savedHP > 0)
            {
                hp.currentHP = Mathf.Min(SaveSystem.Data.savedHP, hp.maxHP);
            }
        }

        // XP & Level
        if (XPManager.Instance != null && SaveSystem.Data.savedLevel > 0)
        {
            XPManager.Instance.SetLevelAndXP(SaveSystem.Data.savedLevel, SaveSystem.Data.savedXP);
        }  

        // Coins
        if (CurrencyManager.Instance != null && SaveSystem.Data.savedCoins >= 0)
        {
            CurrencyManager.Instance.SetCoins(SaveSystem.Data.savedCoins);
        }

        // Keys
        if (player != null)
        {
            var inv = player.GetComponent<PlayerInventory>();
            if (inv != null)
            {
                inv.ClearKeys();
                foreach (var doorID in SaveSystem.Data.savedKeys)
                {
                    var keyData = FindKeyDataByDoorID(doorID);
                    if (keyData != null) inv.AddKey(keyData);
                }
            }
        }

        // Consumables
        if (player != null)
        {
            var inv = player.GetComponent<PlayerInventory>();
            if (inv != null)
            {
                // Clear any runtime data
                inv.ClearConsumables();
                // Rebuild from save
                foreach (var slot in SaveSystem.Data.savedConsumables)
                {
                    inv.AddConsumable(slot.type, slot.count);
                }
                // Restore active (if >= 0)
                int idx = SaveSystem.Data.savedActiveConsumable;
                if (idx >= 0)
                {
                    inv.SetActiveConsumable((ConsumableType)idx);
                }
            }
        }
    }

    void Start()
    {
        // Health
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var hp = player.GetComponent<Health>();
            if (hp != null) hp.currentHP = Mathf.Min(SaveSystem.Data.savedHP, hp.maxHP);
        }

        // XP & Level
        if (XPManager.Instance != null && SaveSystem.Data.savedLevel > 0)
        {
            XPManager.Instance.SetLevelAndXP(SaveSystem.Data.savedLevel, SaveSystem.Data.savedXP);
        }

        // Currency
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.SetCoins(SaveSystem.Data.savedCoins);
        }

        // Keys
        var inv = player.GetComponent<PlayerInventory>();
        if (inv != null)
        {
            //inv.ClearKeys();
            foreach (var doorID in SaveSystem.Data.savedKeys)
            {
                var keyData = FindKeyDataByDoorID(doorID);
                if (keyData != null) inv.AddKey(keyData);
            }
        }
    }

    // Helper to look up ScriptableObject by doorID
    private KeyItemData FindKeyDataByDoorID(string id)
    {
        foreach (var so in Resources.LoadAll<KeyItemData>("KeyItemData"))
        {
            if (so.doorID == id) return so;
        }
        return null;
    }

}
