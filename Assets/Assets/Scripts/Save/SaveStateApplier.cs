using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveStateApplier : MonoBehaviour
{
    private void Awake()
    {
        SaveSystem.Load();
        ApplySavedState();

        // Listen for any scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("[SceneBootstrap] Player already present? " + (FindFirstObjectByType<PlayerInventory>() != null));

    }

    private void OnDestroy()
    {
        // Clean up the callback to avoid leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedApplyState());
    }

    private IEnumerator DelayedApplyState()
    {
        // Wait until PlayerInventory exists in the scene
        while (FindFirstObjectByType<PlayerInventory>() == null)
            yield return null;

        Debug.Log("[SaveStateApplier] Player detected. Applying saved state.");
        ApplySavedState();

        // Refresh UI & reapply gameplay state
        FindFirstObjectByType<InventoryUI>()?.RefreshSlots();

        var am = FindFirstObjectByType<AbilityManager>();
        if (am != null)
        {
            am.InitializeUnlockedAbilities(SaveSystem.Data.savedLevel);
        }

        FindFirstObjectByType<AbilityHotbarUI>()?.RebuildHotbar();
    }

    public void ApplySavedState()
    {
        SaveSystem.Load();
        //Debug.Log("[SaveStateApplier]: APPLYING SAVE STATE");

        // Health
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var hp = player.GetComponent<Health>();
            if (hp != null)
            {
                if (SaveSystem.Data.savedMaxHP > 0)
                    hp.maxHP = SaveSystem.Data.savedMaxHP;

                if (SaveSystem.Data.savedHP > 0)
                    hp.currentHP = Mathf.Min(SaveSystem.Data.savedHP, hp.maxHP);
            }
            /*if (hp != null && SaveSystem.Data.savedHP > 0)
            {
                hp.currentHP = Mathf.Min(SaveSystem.Data.savedHP, hp.maxHP);
            }*/
        }

        // XP & Level
        /*if (XPManager.Instance != null && SaveSystem.Data.savedLevel > 0)
        {
            XPManager.Instance.SetLevelAndXP(SaveSystem.Data.savedLevel, SaveSystem.Data.savedXP);
        }*/

        if (XPManager.Instance != null)
        {
            int lvl = Mathf.Max(SaveSystem.Data.savedLevel, 1);
            int xp = Mathf.Max(SaveSystem.Data.savedXP, 0);
            XPManager.Instance.SetLevelAndXP(lvl, xp);
            //XPManager.Instance.SetLevelAndXP(SaveSystem.Data.savedLevel, SaveSystem.Data.savedXP);
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
                Debug.Log($"[SaveStateApplier] Restoring {SaveSystem.Data.savedKeys.Count} saved key(s).");
                inv.ClearKeys();
                foreach (var doorID in SaveSystem.Data.savedKeys)
                {
                    var keyData = FindKeyDataByDoorID(doorID);
                    if (keyData != null) 
                    { 
                        inv.AddKey(keyData);
                        Debug.Log($"[SaveStateApplier] Added key: {keyData.keyName} (doorID: {doorID})");
                    }
                    else
                    {
                        Debug.LogWarning($"[SaveStateApplier] Could not find KeyData for doorID: {doorID}");
                    }
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

        // UI refresh
        var inventoryUI = FindFirstObjectByType<InventoryUI>();
        if (inventoryUI != null)
        {
            Debug.Log("[SaveStateApplier]: Forcing InventoryUI refresh...");
            inventoryUI.RefreshSlots();
        }
        else
        {
            //Debug.LogWarning("[SaveStateApplier] InventoryUI not found in scene.");
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
