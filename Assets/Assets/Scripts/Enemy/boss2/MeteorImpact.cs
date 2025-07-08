using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MeteorImpact : MonoBehaviour
{
    public int damage = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var h = other.GetComponent<Health>();
        if (h != null) h.TakeDamage(damage);
    }
}
