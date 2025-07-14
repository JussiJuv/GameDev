using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class BossGateController : MonoBehaviour
{
    [Header("Colliders")]
    public Collider2D blockingCollider;
    public Collider2D entryTriggerCollider;

    [Header("Fade")]
    public BossRoomFade roomFade;

    [Header("Boss Activation")]
    [Tooltip("The boss script to call ActivateBoss() on")]
    public MirrorBinderController bossController;
    [Tooltip("Seconds to wait after gate locks before boss can move/attack")]
    public float bossActivationDelay = 1f;

    [Header("Events")]
    [Tooltip("Invoked when the gate is locked (fight begins)")]
    public UnityEvent onGateLocked;
    [Tooltip("Invoked when the gate is unlocked (boss defeated)")]
    public UnityEvent onGateUnlocked;

    private bool isLocked = false;

    private void Reset()
    {
        var cols = GetComponents<Collider2D>();
        foreach (var c in cols)
        {
            if (c.isTrigger) entryTriggerCollider = c;
            else blockingCollider = c;
        }
    }

    private void Awake()
    {
        // Start with the exit unblocked
        if (blockingCollider != null) blockingCollider.enabled = false;
        // Trigger is enabled so we know when the player enters
        if (entryTriggerCollider != null) entryTriggerCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isLocked && collision.CompareTag("Player"))
            LockGate();
    }

    /// <summary>
    /// Call to lock the gate
    /// </summary>
    public void LockGate()
    {
        if (isLocked) return;
        isLocked = true;

        if (entryTriggerCollider != null) entryTriggerCollider.enabled = false;
        if (blockingCollider != null) blockingCollider.enabled = true;

        roomFade?.PlayFadeIn();

        onGateLocked?.Invoke();

        // Wake the boss after delay
        if (bossController != null)
            StartCoroutine(DelayedBossActivation());
        else
            Debug.LogError("[BossGateController]: No bossController assigned");
    }

    private IEnumerator DelayedBossActivation()
    {
        yield return new WaitForSeconds(bossActivationDelay);
        bossController.ActivateBoss();
    }

    /// <summary>
    /// Call to unlock the gate
    /// </summary>
    public void UnlockGate()
    {
        if (!isLocked) return;
        isLocked = false;

        if (blockingCollider != null) blockingCollider.enabled = false;

        onGateUnlocked?.Invoke();
    }
}
