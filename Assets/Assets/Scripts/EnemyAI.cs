using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Enable or disable enemy movement")]
    public bool canMove = true;

    [Tooltip("Movement speed in units/sec")]
    public float moveSpeed = 2f;

    [Tooltip("How far to patrol from starting point")]
    public float patrolRadius = 3f;

    private Vector3 startPos;
    private Vector3 targetPos;

    void Awake()
    {
        startPos = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        if (!canMove)
            return;

        // Move toward target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // When close, pick new point
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            PickNewTarget();
    }

    void PickNewTarget()
    {
        // Random point in a circle around start
        Vector2 rnd = Random.insideUnitCircle * patrolRadius;
        targetPos = startPos + new Vector3(rnd.x, rnd.y, 0f);
    }
}
