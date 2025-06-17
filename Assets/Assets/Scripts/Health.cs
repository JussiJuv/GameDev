using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Health : MonoBehaviour
{
    [Tooltip("Maximum hit points")]
    public int maxHP = 3;

    [Tooltip("Invoked when HP reaches zero")]
    public UnityEvent OnDeath;

    private int currentHP;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP = Mathf.Max(currentHP - amount, 0);
        Debug.Log($"{name} took {amount} damage, {currentHP}/{maxHP} HP left");

        if (currentHP == 0)
            Die();
    }

    private void Die()
    {
        // Let any listeners run (e.g. play VFX, drop loot)
        OnDeath?.Invoke();

        // Default behavior: destroy after a short delay
        Destroy(gameObject);
    }
}
