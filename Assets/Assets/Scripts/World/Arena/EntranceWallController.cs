using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EntranceWallController : MonoBehaviour
{
    [Header("Blocking")]
    [Tooltip("The non-trigger collider that blocks the player")]
    public Collider2D blockCollider;

    [Tooltip("SpriteRenderer for wall graphic")]
    public SpriteRenderer wallSprite;

    [Header("Trigger (this gameobject)")]
    [Tooltip("Trigger collider on this object (isTrigger = true)")]
    public Collider2D triggerCollider;

    private bool spawned = false;

    private void Awake()
    {
        // Start hidden
        if (wallSprite != null) wallSprite.enabled = false;
        if (blockCollider != null) blockCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (spawned) return;
        if (!collision.CompareTag("Player")) return;

        // Spawn the wall
        spawned = true;
        if (wallSprite != null) wallSprite.enabled = true;
        if (blockCollider != null) blockCollider.enabled = true;

        // disable trigger so it only fires once
        if (triggerCollider != null) triggerCollider.enabled = false;
     }
}