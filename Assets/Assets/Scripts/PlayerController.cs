using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Units per second")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
       // Cache the Rigidbody2D so we can use it in FixedUpdate
       rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Read raw input axes
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        Vector2 newPos = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }
}
