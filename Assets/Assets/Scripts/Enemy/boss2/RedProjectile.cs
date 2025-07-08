using UnityEngine;

public class RedProjectile : MonoBehaviour
{
    public float speed = 4f;
    public float rotateSpeed = 200f;
    public int damage = 1;
    Transform target;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        var dir = (Vector2)(target.position - transform.position);
        var angle = Vector2.SignedAngle(transform.right, dir);
        transform.Rotate(0, 0, Mathf.Clamp(angle, -rotateSpeed, rotateSpeed) * Time.deltaTime);
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        var h = col.GetComponent<Health>();
        if (h != null) h.TakeDamage(damage);
        Destroy(gameObject);
    }
}
