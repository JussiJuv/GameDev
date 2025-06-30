using UnityEngine;

public class BossRoomEntrance : MonoBehaviour
{
    [Header("Room Setup")]
    public GameObject bossRoomRoot;

    [Header("Barrier Collider Setup")]
    [Tooltip("This solid collider blocks exit. Should be disabled at start.")]
    public Collider2D barrierCollider;

    [Tooltip("This trigger collider detects player entry. Should be enabled at start.")]
    public Collider2D triggerCollider;

    [Header("Visuals")]
    public ParticleSystem doorEffect;

    public BossController bossController;

    private bool isRoomRevealed = false;

    void Awake()
    {
        // Hide the boss room until revealed
        if (bossRoomRoot != null)
            bossRoomRoot.SetActive(false);

        // Ensure barrier is off, trigger is on
        if (barrierCollider != null)
            barrierCollider.enabled = false;

        if (triggerCollider != null)
            triggerCollider.enabled = true;

        // Make sure AI is off at start
        if (bossController != null)
        {
            bossController.canActivate = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isRoomRevealed) return;
        if (!other.CompareTag("Player")) return;
        EnterBossRoom();
    }

    void EnterBossRoom()
    {
        isRoomRevealed = true;

        // Reveal the room
        //bossRoomRoot?.SetActive(true);
        if (bossRoomRoot != null)
        {
            bossRoomRoot.SetActive(true);
        }

        /*// Stop or hide the mist effect
        if (doorEffect != null)
            doorEffect.Stop();*/

        // Switch colliders: block exit, stop further triggers
        if (barrierCollider != null)
            barrierCollider.enabled = true;

        if (triggerCollider != null)
            triggerCollider.enabled = false;

        // Turn the boss on
        if (bossController != null)
        {
            bossController.ActivateBoss();
        }
    }

    /// <summary> Call this when the boss dies from BossController </summary>
    public void ReleaseGate()
    {
        // Stop or hide the mist effect
        if (doorEffect != null)
            doorEffect.Stop();

        // Disable the barrier so the player can exit
        if (barrierCollider != null)
        {
            barrierCollider.enabled = false;
        }
    }
}
