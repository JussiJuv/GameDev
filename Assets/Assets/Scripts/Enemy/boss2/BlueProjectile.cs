using UnityEngine;

public class BlueProjectile : MonoBehaviour
{
    private float speed = 5f;
    private int damage = 1;
    private Vector2 direction;

    /// <summary>
    /// Initializes direction, speed, and damage.
    /// </summary>
    public void Init(Vector2 dir, float spd, int dmg)
    {
        direction = dir;
        speed = spd;
        damage = dmg;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        var h = other.GetComponent<Health>();
        if (h != null)
            h.TakeDamage(damage);

        Destroy(gameObject);
    }
}
