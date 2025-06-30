using UnityEngine;

public class SaveStateApplier : MonoBehaviour
{
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
