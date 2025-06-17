using UnityEngine;

public class Health : MonoBehaviour
{
    [Tooltip("Starting HP")]
    public int maxHealth = 1;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage, now at {currentHealth} HP");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // For now, just destroy the GameObject
        Destroy(gameObject);
    }
}
