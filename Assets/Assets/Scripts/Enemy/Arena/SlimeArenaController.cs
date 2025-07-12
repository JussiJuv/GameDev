using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SlimeArenaController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed when player in range")]
    public float moveSpeed = 2f;

    [Header("Attack")]
    [Tooltip("How close the player must be before the enemy attacks")]
    public float attackRange = 1f;

    [Tooltip("Damage dealt per hit")]
    public int attackDamage = 1;

    [Tooltip("Seconds between each attack attempt")]
    public float attackCooldown = 1f;

    private Transform playerT;
    private Rigidbody2D rb;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var p = GameObject.FindWithTag("Player");
        if (p != null) playerT = p.transform;
        else Debug.LogError("[SlimeArenaController]: No Player tagged object in scene");
    }

    private void FixedUpdate()
    {
        if (playerT == null) return;

        float dist = Vector2.Distance(transform.position, playerT.position);

        if (dist <= attackRange)
            TryAttack();
        else
        {
            Vector2 target = Vector2.MoveTowards(
                rb.position,
                playerT.position,
                moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(target);
        }
    }

    /*private void ChasePlayer()
    {
        // run straight at the player
        transform.position = Vector3.MoveTowards(
            transform.position,
            playerT.position,
            moveSpeed * Time.deltaTime);
    }*/

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        var h = playerT.GetComponent<Health>();
        if (h != null) h.TakeDamage(attackDamage);
    }
}
