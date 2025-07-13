using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class SaveSystem
{
    private static string SAVE_FILE => Path.Combine(Application.persistentDataPath, "savegame.json");
    public static SaveData Data { get; private set; }

    /// <summary>
    /// Call once at game startup to load or initialize save data
    /// </summary>
    public static void Load()
    {
        if (File.Exists(SAVE_FILE))
        {
            string json = File.ReadAllText(SAVE_FILE);
            Data = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            // No save found, create a new save
            Data = new SaveData()
            {
                lastScene = SceneManager.GetActiveScene().name,
                lastCheckpointID = "Demo_Start",
                savedHP = -1,
                savedLevel = -1,
                savedXP = -1,
                savedCoins = -1
            };
            Save();
        }
    }

    /// <summary>
    /// Write the current data back out to disk
    /// </summary>
    public static void Save()
    {
        // Always update current scene name before writing
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "UI")
            Data.lastScene = sceneName;

        // Player health
        var playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
        Data.savedHP = playerHealth != null
            ? playerHealth.currentHP
            : playerHealth.maxHP;

        // XP & Level
        if (XPManager.Instance != null)
        {
            Data.savedLevel = XPManager.Instance.currentLevel;
            Data.savedXP = XPManager.Instance.currentXP;
        }

        // Currency
        if (CurrencyManager.Instance != null)
        {
            Data.savedCoins = CurrencyManager.Instance.Coins;
        }

        // Keys
        var inv = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        Data.savedKeys.Clear();
        foreach (var key in inv.Keys)
        {
            Data.savedKeys.Add(key.doorID);
        }

        // Consumables
        Data.savedConsumables.Clear();
        foreach (var slot in inv.Consumables)
        {
            Data.savedConsumables.Add(slot);
        }
        Data.savedActiveConsumable = inv.ActiveConsumable.HasValue
            ? (int)inv.ActiveConsumable.Value
            : -1;

        string json = JsonUtility.ToJson(Data, prettyPrint: true);
        File.WriteAllText(SAVE_FILE, json);
    }

    /// <summary>
    /// Record a new checkpoint ID and persist immediately
    /// </summary>
    public static void SetCheckpoint(string checkpointID)
    {
        Data.lastCheckpointID = checkpointID;
        Save();
    }
}
