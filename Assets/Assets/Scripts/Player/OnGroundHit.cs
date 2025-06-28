using UnityEngine;

public class OnGroundHit : MonoBehaviour
{
    public GameObject hitEffectPrefab;
    public float damageRadius = 1f;
    public int damageAmount = 10;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (((1 << col.gameObject.layer) &
             LayerMask.GetMask("Ground")) == 0) return;

        // spawn effect
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        // damage enemies in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            damageRadius,
            LayerMask.GetMask("Enemies")
        );
        foreach (var c in hits)
            c.GetComponent<Health>()?.TakeDamage(damageAmount);

        Destroy(gameObject);
    }
}
