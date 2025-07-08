using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RedProjectile : MonoBehaviour
{
    public float speed = 4f;
    public int damage = 1;
    private Vector2 direction;

    /// <summary>
    /// Initializes the projectile's direction, speed, and damage.
    /// </summary>
    /// <param name="target">Transform of the player (or desired target)</param>
    /// <param name="spd">Projectile speed</param>
    /// <param name="dmg">Damage on hit</param>
    public void InitStraight(Transform target, float spd, int dmg)
    {
        if (target != null)
            direction = (target.position - transform.position).normalized;
        else
            direction = transform.right; // fallback

        speed = spd;
        damage = dmg;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Only damage the player or any other desired tags
        if (!col.CompareTag("Player"))
            return;

        var h = col.GetComponent<Health>();
        if (h != null)
            h.TakeDamage(damage);

        Destroy(gameObject);
    }
}
