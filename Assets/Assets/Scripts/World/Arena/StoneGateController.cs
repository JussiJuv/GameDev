using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StoneGateController : MonoBehaviour
{
    [Header("Gate Settings")]
    [Tooltip("Animator with an 'OpenGate' trigger")]
    public Animator animator;
    public string openTrigger = "OpenGate";

    [Tooltip("The collider that prevents passage when closed")]
    public Collider2D passCollider;

    private bool isOpen = false;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Call this once all waves are done
    /// </summary>
    public void OpenGate()
    {
        if (isOpen) return;
        isOpen = true;

        // Play the opening animation
        animator.SetTrigger(openTrigger);

        // Let the player walk through
        if (passCollider != null) passCollider.enabled = false;
    }

}
