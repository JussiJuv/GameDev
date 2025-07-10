using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ImpProjectile : MonoBehaviour
{
    [Tooltip("Speed of the projectile")]
    public float speed = 6f;

    [Tooltip("Damage to deal on hit")]
    public int damage = 1;

    [Tooltip("Seconds before auto?destroy")]
    public float lifetime = 5f;

    private Vector2 direction;

    /// <summary>
    /// Call immediately after Instantiate.
    /// </summary>
    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var h = other.GetComponent<Health>();
        if (h != null) h.TakeDamage(damage);

        Destroy(gameObject);
    }
}
