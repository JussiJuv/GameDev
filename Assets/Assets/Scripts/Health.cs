using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Health : MonoBehaviour
{
    [Tooltip("Maximum hit points")]
    public int maxHP = 3;

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
    //public AudioClip deathSFX;

    //private AudioSource audioSource;
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

        //audioSource = GetComponent<AudioSource>();
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
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Play SFX if assigned
        if (deathClip != null)
        {
            AudioSource.PlayClipAtPoint(deathClip, transform.position);
        }
        /*if (CompareTag("Player") && deathSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSFX);
        }*/

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

        // If this is the Player, invoke OnPlayerDeath as well
        if (CompareTag("Player")) OnPlayerDeath?.Invoke();

        // Finally destroy this GameObject after a delay
        Destroy(gameObject, destroyDelay);

        /*// Let any listeners run (e.g. play VFX, drop loot)
        OnDeath?.Invoke();

        // Default behavior: destroy after a short delay
        Destroy(gameObject);*/
    }
}
