using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int Coins { get; private set; }

    // Fired whenever coins change (for UI later)
    public event Action<int> OnCoinsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

 
    // Add coins and notify listeners.
    public void AddCoins(int amount)
    {
        Coins += amount;
        Debug.Log($"Collected {amount} coins. Total: {Coins}");
        OnCoinsChanged?.Invoke(Coins);
    }

    // Spend coins (returns true if enough).
    public bool SpendCoins(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        OnCoinsChanged?.Invoke(Coins);
        return true;
    }
}
