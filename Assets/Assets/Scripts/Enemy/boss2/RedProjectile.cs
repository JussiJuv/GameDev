using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RedProjectile : MonoBehaviour
{
    public float speed = 4f;
    public int damage = 1;
    private Vector2 direction;
    [Min(0)] public float lifetime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Initializes the projectile's direction, speed, and damage.
    /// </summary>
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1) If it hits the Player, damage as before:
        if (collision.CompareTag("Player"))
        {
            var h = collision.GetComponent<Health>();
            if (h != null) h.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // If it hits a player arrow or wall, destroy itself:
        if (collision.CompareTag("PlayerProjectile"))
        {
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
            return;
        }
    }

    /*private void OnTriggerEnter2D(Collider2D col)
    {
        // Only damage the player or any other desired tags
        if (!col.CompareTag("Player"))
            return;

        var h = col.GetComponent<Health>();
        if (h != null)
            h.TakeDamage(damage);

        Destroy(gameObject);

        if (col.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
            return;
        }
    }*/
}
