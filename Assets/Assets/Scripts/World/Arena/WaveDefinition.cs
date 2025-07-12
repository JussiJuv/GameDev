using UnityEngine;

[System.Serializable]
public class WaveDefinition
{
    [Tooltip("Which enemies to spawn, and from which spawnPoint")]
    public SpawnEntry[] spawns;
    [Tooltip("Spacing between each individual spawn in this wave")]
    public float spawnDelay = 0.5f;
}