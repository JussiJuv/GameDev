using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EntranceWallController : MonoBehaviour
{
    [Header("Blocking Collider & Sprite")]
    [Tooltip("The non-trigger collider that blocks the player")]
    public Collider2D blockCollider;

    [Tooltip("SpriteRenderer for wall")]
    public SpriteRenderer wallSprite;

    [Tooltip("Trigger collider on this object (isTrigger = true)")]
    public Collider2D triggerCollider;

    bool spawned = false;

    private void Awake()
    {
        // Starts hidden
        if (wallSprite != null) wallSprite.enabled = false;
        if (blockCollider != null) blockCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (spawned) return;
        if (!collision.CompareTag("Player")) return;

        SpawnWall();
    }

    /// <summary>
    /// Makes wall appear and block the doorway
    /// </summary>
    public void SpawnWall()
    {
        if (spawned) return;
        spawned = true;

        if (wallSprite != null) wallSprite.enabled = true;
        if (blockCollider != null) blockCollider.enabled = true;
        if (triggerCollider != null) triggerCollider.enabled = false;
    }
}