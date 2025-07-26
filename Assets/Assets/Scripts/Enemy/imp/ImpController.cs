using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ImpController : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform patrolPointA;
    public Transform patrolPointB;
    [Tooltip("Maximum random offset applied to each patrol target")]
    public float patrolOffsetRange = 0.5f;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Attack")]
    public float attackRange = 5f;
    public int shotsPerVolley = 3;
    public float timeBetweenShots = 0.5f;
    public float timeBetweenVolleys = 2f;

    [Header("Projectile")]
    public Transform firePoint;
    public GameObject impProjectilePrefab;

    [Header("Audio")]
    public AudioClip attackSFX;

    private Transform player;
    private Animator anim;
    private Vector3 nextPatrol;
    private bool isAttacking;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player")?.transform;

        var health = GetComponent<Health>();
        health.OnDeath.AddListener(OnDeath);
        health.OnHealthChanged.AddListener((curr, max) => anim.SetTrigger("Hurt"));

        if (patrolPointA == null || patrolPointB == null)
            Debug.LogWarning("[ImpController] Patrol points not set on " + name);

        // Choose whichever point is farther as the first target
        float dA = Vector3.Distance(transform.position, patrolPointA.position);
        float dB = Vector3.Distance(transform.position, patrolPointB.position);
        /*nextPatrol = (dA < dB)
            ? patrolPointB.position
            : patrolPointA.position;*/

        nextPatrol = (dA < dB)
            ? GetRandomizedPoint(patrolPointB)
            : GetRandomizedPoint(patrolPointA);
    }

    void Update()
    {
        if (isAttacking) return;

        // Make sure player still exists
        if (player == null) return;

        // Distance to current patrol goal and to player
        float distToGoal = Vector3.Distance(transform.position, nextPatrol);
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        //Debug.Log($"[Imp] distToGoal={distToGoal:F2}, distToPlayer={distToPlayer:F2}");

        if (distToGoal > 0.1f)
        {
            // Still moving toward the goal
            Patrol();
        }
        else
        {
            // We’ve reached the patrol point—only attack if player is in range
            if (distToPlayer <= attackRange)
            {
                Debug.Log("[Imp] Player in range at patrol point – starting volley");
                StartCoroutine(AttackVolley());
            }
            else
            {
                anim.SetBool("IsMoving", false);
            }
        }
    }

    private void Patrol()
    {
        anim.SetBool("IsMoving", true);
        Vector3 dir = (nextPatrol - transform.position).normalized;

        // Flip sprite
        if (dir.x > 0.01f) spriteRenderer.flipX = true;
        else if (dir.x < -0.01f) spriteRenderer.flipX = false;

        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
    }

    private IEnumerator AttackVolley()
    {
        isAttacking = true;
        anim.SetBool("IsMoving", false);
        Debug.Log("[Imp] AttackVolley started");

        if (player == null) yield break;

        for (int i = 0; i < shotsPerVolley; i++)
        {
            // check if the player still exists
            if (player == null) break;
            anim.SetTrigger("Attack");

            // compute direction to player
            Vector2 dir = (player.position - firePoint.position).normalized;

            // spawn & init projectile
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180f;
            var go = Instantiate(impProjectilePrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
            go.GetComponent<ImpProjectile>()?.Init(dir);

            if (attackSFX != null)
            {
                float pitch = Random.Range(0.9f, 1.1f);
                AudioManager.Instance.PlaySFX(attackSFX, pitch);
            }

            yield return new WaitForSeconds(timeBetweenShots);
        }

        yield return new WaitForSeconds(timeBetweenVolleys);
        /*nextPatrol = (nextPatrol == patrolPointA.position)
                 ? patrolPointB.position
                 : patrolPointA.position;
        Debug.Log("[Imp] AttackVolley finished – nextPatrol set");*/

        if (Vector3.Distance(nextPatrol, patrolPointA.position) < 0.1f)
            nextPatrol = GetRandomizedPoint(patrolPointB);
        else
            nextPatrol = GetRandomizedPoint(patrolPointA);

        isAttacking = false;
    }

    private Vector3 GetRandomizedPoint(Transform point)
    {
        Vector3 basePos = point.position;
        float offsetX = Random.Range(-patrolOffsetRange, patrolOffsetRange);
        float offsetY = Random.Range(-patrolOffsetRange, patrolOffsetRange);
        return new Vector3(basePos.x + offsetX, basePos.y + offsetY, basePos.z);
    }

    /// <summary>
    /// Call this from an animation event in your Hurt/Death clips if needed:
    /// </summary>
    public void OnHurt() => anim.SetTrigger("Hurt");

    public void OnDeath()
    {
        anim.SetTrigger("Die");
        // disable further logic
        enabled = false;
        // optionally destroy after a delay matching the death clip
        Destroy(gameObject, 1.0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
