using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Health : MonoBehaviour
{
    [Tooltip("Maximum hit points")]
    public int maxHP = 3;

    [Tooltip("XP awarded when this entity dies (set 0 for Player or non-XP entities)")]
    public int xpValue = 0;

    [Tooltip("Seconds to wait before actually destroying the GameObject")]
    public float destroyDelay = 0.5f;

    [Tooltip("Optional VFX prefab to spawn on death")]
    public GameObject deathEffectPrefab;

    [Header("Events")]
    [Tooltip("Invoked when HP reaches zero")]
    public UnityEvent OnDeath;

    [Tooltip("Called only if this health belongs to the Player")]
    public UnityEvent OnPlayerDeath;

    [Header("Audio")]
    [Tooltip("SFX to play when player dies")]
    public AudioClip deathClip;

    private int currentHP;
    private Collider2D col;
    private Behaviour[] disableOnDeath;
    private bool isDead = false;

    void Awake()
    {
        currentHP = maxHP;
        col = GetComponent<Collider2D>();

        // Gather components to disable on death
        disableOnDeath = GetComponents<Behaviour>();

    }

    public void TakeDamage(int amount)
    {
        // If already dead, ignore further damage
        if (isDead) return;

        currentHP = Mathf.Max(currentHP - amount, 0);
        Debug.Log($"{name} took {amount} damage, {currentHP}/{maxHP} HP left");

        if (currentHP == 0)
            Die();
    }

    private void Die()
    {
        isDead = true;

        // Disable collisions so nothing else hits the dead object
        if (col != null)
        {
            col.enabled = false;
        }


        // Spawn VFX if assigned
        if (deathEffectPrefab != null)
        {
            Debug.Log("Playing death VFX");
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("VFX Prefab not found");
        }

        // Play SFX if assigned
        if (deathClip != null)
        {
            //AudioSource.PlayClipAtPoint(deathClip, transform.position);
            //sfxAudioSource.PlayOneShot(deathClip);
            AudioManager.Instance.PlaySFX(deathClip);
        }

        // Disable any MonoBehaviours
        foreach (var comp in disableOnDeath)
        {
            if (comp != this)   // Keep health script
            {
                comp.enabled = false;
            }
        }

        // Invoke the general OnDeath event
        OnDeath?.Invoke();

        // Award XP if this is an enemy (xpValue > 0)
        if (xpValue > 0 && XPManager.Instance != null)
        {
            XPManager.Instance.AddXP(xpValue);
            Debug.Log($"Awarded {xpValue} XP for defeating {name}");
        }

        // If this is the Player, invoke OnPlayerDeath as well
        if (CompareTag("Player")) OnPlayerDeath?.Invoke();

        // Finally destroy this GameObject after a delay
        Destroy(gameObject, destroyDelay);

    }
}
