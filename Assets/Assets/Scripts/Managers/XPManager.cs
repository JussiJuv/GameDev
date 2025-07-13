using UnityEngine;
using System;

// Singleton to track XP, levels, and start level-up events.
public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    [Header("Level Settings")]
    public int startingLevel = 1;
    public int[] xpThresholds;

    [Header("Runntime Values")]
    public int currentLevel { get; private set; }
    public int currentXP { get; private set; }
    public int xpToNextLevel { get; private set; }

    public event Action<int> OnLevelUp;
    public event Action<int, int> OnXPChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentLevel = startingLevel;
        currentXP = 0;
        xpToNextLevel = GetXPThreshold(currentLevel);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);

        // This is horrible
        // Override with saved data if it exists
        if (SaveSystem.Data.savedLevel > 0)
        {
            Debug.Log($"[XPManager] Loading saved level {SaveSystem.Data.savedLevel}, XP {SaveSystem.Data.savedXP}");
            SetLevelAndXP(SaveSystem.Data.savedLevel, SaveSystem.Data.savedXP);
        }
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        //Debug.Log($"Gained {amount} XP. Total XP: {currentXP} / {xpToNextLevel}");
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        xpToNextLevel = GetXPThreshold(currentLevel);
        //Debug.Log($"Leveled up! New Level: {currentLevel}");
        OnLevelUp?.Invoke(currentLevel);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }

    private int GetXPThreshold(int level)
    {
        if (xpThresholds == null || xpThresholds.Length == 0)
        {
            Debug.LogWarning("XP Thresholds not set! Returning default 100.");
            return 100;
        }

        int index = level - 1;
        if (index < xpThresholds.Length) return xpThresholds[index];

        return xpThresholds[xpThresholds.Length - 1]; // Max threshold for overflow levels
    }

    public void SetLevelAndXP(int level, int xp)
    {
        currentLevel = level;
        currentXP = xp;
        xpToNextLevel = GetXPThreshold(level);
        OnXPChanged?.Invoke(currentXP, xpToNextLevel);
    }

}
