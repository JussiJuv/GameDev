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
}
