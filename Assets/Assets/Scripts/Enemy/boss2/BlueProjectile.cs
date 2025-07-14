using UnityEngine;

public class BlueProjectile : MonoBehaviour
{
    private float speed = 5f;
    private int damage = 1;
    private Vector2 direction;
    [Min(0)] public float lifetime = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Init(Vector2 dir, float spd, int dmg)
    {
        direction = dir;
        speed = spd;
        damage = dmg;
    }

    void Update()
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
}