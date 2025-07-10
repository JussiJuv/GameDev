using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Tooltip("Seconds before arrow auto-destroys")]
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore imp projectiles
        if (other.GetComponent<ImpProjectile>() != null) return;

        var health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(1);
        }
        Destroy(gameObject); // Destroy on hit anything with health
    }
}
