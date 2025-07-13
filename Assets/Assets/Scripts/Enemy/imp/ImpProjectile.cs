using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ImpProjectile : MonoBehaviour
{
    [Tooltip("Speed of the projectile")]
    public float speed = 6f;

    [Tooltip("Damage to deal on hit")]
    public int damage = 1;

    [Tooltip("Seconds before auto-destroy")]
    public float lifetime = 5f;

    [Tooltip("Which layers should stop this projectile on hit?")]
    public LayerMask destructibleLayers;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if we hit player, deal damage
        if (collision.CompareTag("Player"))
        {
            var h = collision.GetComponent<Health>();
            if (h != null) h.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // if we hit anything on destructibleLayers, just destroy
        if (((1 << collision.gameObject.layer) & destructibleLayers) != 0) 
            Destroy(gameObject);
    }

    /*void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var h = collision.GetComponent<Health>();
        if (h != null) h.TakeDamage(damage);

        Destroy(gameObject);
    }*/
}
