using UnityEngine;

[System.Serializable]
public class SpawnEntry
{
    [Tooltip("SpawnPoint VFX object (must have Animator + SpriteRenderer)")]
    public Transform spawnPoint;
    [Tooltip("Enemy prefab to instantiate")]
    public GameObject prefab;
}