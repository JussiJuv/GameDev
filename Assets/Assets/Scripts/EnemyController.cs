using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Can this enemy move at all?")]
    public bool canMove = true;

    [Tooltip("Patrol speed (units/sec)")]
    public float patrolSpeed = 2f;

    [Tooltip("Chase speed when player in range")]
    public float chaseSpeed = 2f;

    [Tooltip("How far from its start position to patrol")]
    public float patrolRadius = 3f;

    [Header("Detection & Attack")]
    [Tooltip("How close the player must be before the enemy starts chasing")]
    public float chaseRange = 5f;

    [Tooltip("How close the player must be before the enemy attacks")]
    public float attackRange = 1f;

    [Tooltip("Damage dealt per hit")]
    public int attackDamage = 1;

    [Tooltip("Seconds between each attack attempt")]
    public float attackCooldown = 1f;

    private Vector3 startPos;
    private Vector3 patrolTarget;
    private Transform playerT;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        startPos = transform.position;
        PickPatrolTarget();

        var playerGo = GameObject.FindWithTag("Player");
        if (playerGo != null)
        {
            playerT = playerGo.transform;
        }
        else
        {
            Debug.LogError("EnemyController: No GameObject tagged 'Player' found in scene.");
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!canMove || playerT == null)
        {
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, playerT.position);
        if (distToPlayer <= attackRange)
        {
            TryAttack();
        }
        else if (distToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        // Move toward current patrolTarget
        transform.position = Vector3.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);

        // If we reached the patrol point, pick a new one
        if (Vector2.Distance(transform.position, patrolTarget) < 0.1f) PickPatrolTarget();
    }

    private void PickPatrolTarget()
    {
        Vector2 rnd = Random.insideUnitCircle * patrolRadius;
        patrolTarget = startPos + new Vector3(rnd.x, rnd.y, 0f);
    }

    private void ChasePlayer()
    {
        // Move directly toward player
        transform.position = Vector3.MoveTowards(transform.position, playerT.position, chaseSpeed * Time.deltaTime);
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        // Deal damage if player has a Health component
        var playerHealth = playerT.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            // TODO: trigger attack animation here
        }
    }

    // visualize detection ranges in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
