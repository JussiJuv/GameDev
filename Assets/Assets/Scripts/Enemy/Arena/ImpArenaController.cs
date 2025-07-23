using UnityEngine;
using System.Linq;
using System.Collections;

[RequireComponent(typeof(Health))]
public class ImpArenaController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Attack")]
    public int shotsPerVolley = 3;
    public float timeBetweenShots = 0.5f;
    public float timeBetweenVolleys = 2f;

    [Header("Projectile")]
    public Transform firePoint;
    public GameObject impProjectilePrefab;

    private Transform[] patrolPoints;
    private Vector3 nextTarget;
    private bool isAttacking;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        health.OnDeath.AddListener(OnDeath);
    }

    private void Start()
    {
        if (health != null)
        {
            health.OnHealthChanged.AddListener((curr, max) => anim.SetTrigger("Hurt"));
        }

        // find all four spawn locations
        var locParent = GameObject.Find("Arena/ImpLocations");
        if (locParent == null)
        {
            Debug.LogError("[ImpArenaController]: Could not find Arena/ImpLocation in scene");
            enabled = false;
            return;
        }

        patrolPoints = locParent.GetComponentsInChildren<Transform>()
            .Where(t => t != locParent.transform).ToArray();

        if (patrolPoints.Length < 2)
        {
            Debug.LogError("[ImpArenaController]: Need at least 2 patrol points");
            enabled = false;
            return;
        }

        // Pick two distinct random indices
        int i = Random.Range(0, patrolPoints.Length);
        int j;
        do { j = Random.Range(0, patrolPoints.Length); }
        while (j == i);

        Vector3 a = patrolPoints[i].position;
        Vector3 b = patrolPoints[j].position;
        // Start heading toward the closer point
        nextTarget = (Vector3.Distance(transform.position, a) < Vector3.Distance(transform.position, b))
            ? b
            : a;
    }

    private void Update()
    {
        if (isAttacking || health.currentHP <= 0) return;

        // Move toward next target
        Vector3 dir = (nextTarget - transform.position).normalized;
        // flip sprite
        if (dir.x > 0.01f) spriteRenderer.flipX = true;
        else if (dir.x < -0.01f) spriteRenderer.flipX = false;

        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);

        // Check arrival
        if (Vector3.Distance(transform.position, nextTarget) < 0.1f)
            StartCoroutine(AttackVolley());
    }

    private IEnumerator AttackVolley()
    {
        isAttacking = true;
        anim.SetBool("IsMoving", false);

        yield return null;

        for (int k = 0; k < shotsPerVolley; k++)
        {
            anim.SetTrigger("Attack");

            // Fire straight at player
            var player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
            {
                Vector2 aim = (player.position - firePoint.position).normalized;
                float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg + 180f;
                var proj = Instantiate(impProjectilePrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
                proj.GetComponent<ImpProjectile>()?.Init(aim);
            }
            yield return new WaitForSeconds(timeBetweenShots);
        }
        yield return new WaitForSeconds(timeBetweenVolleys);

        // Find next point
        Vector3 a = patrolPoints.First(t => Vector3.Distance(t.position, nextTarget) > 0.1f).position;
        nextTarget = a;

        anim.SetBool("IsMoving", true);
        isAttacking = false;
    }

    public void OnHurt() => anim.SetTrigger("Hurt");

    private void OnDeath()
    {
        anim.SetTrigger("Die");
        enabled = false;
        Destroy(gameObject, 1.0f);
    }
}
