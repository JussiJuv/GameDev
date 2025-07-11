using System;
using System.Collections.Generic;


[Serializable]
public class SaveData
{
    public string lastScene;
    public string lastCheckpointID;

    // Player state
    public int savedHP;
    public int savedLevel;
    public int savedXP;

    // Currency
    public int savedCoins;

    // Keys: store doorIDs (strings)
    public List<string> savedKeys = new List<string>();

    // Consumables
    public List<InventorySlot> savedConsumables = new List<InventorySlot>();
    // - 1 means no active consumable
    public int savedActiveConsumable = -1;

    // Opened chests
    public List<string> openedChests = new List<string>();
}
