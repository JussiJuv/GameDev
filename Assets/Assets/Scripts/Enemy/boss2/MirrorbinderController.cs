using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class MirrorBinderController : MonoBehaviour
{
    [Header("Phase One Settings (100%?66%)")]
    [Min(1)] public int volleysBeforeAbility = 3;
    [Min(1)] public int shotsPerVolley = 3;
    [Min(0)] public float shotInterval = 0.5f;
    [Min(0)] public float volleyCooldown = 1.5f;

    [Header("Projectile Settings")]
    public int regularProjectileDamage = 1;
    [Min(0)] public float regularProjectileSpeed = 5f;
    public int abilityProjectileDamage = 1;
    [Min(0)] public float abilityProjectileSpeed = 2f;

    [Header("Ability One: Circular Blast")]
    [Min(1)] public int circularShotCount = 12;
    [Min(0)] public float postAbilityCooldown = 2f;

    [Header("Prefabs")]
    public GameObject projectileBluePrefab;

    private enum State { Volley, Ability }
    private State currentState = State.Volley;
    private int volleyCounter = 0;
    private Health health;
    private Animator anim;
    private Transform player;

    private void Awake()
    {
        health = GetComponent<Health>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
        else
            Debug.LogWarning("MirrorBinderController: Player tag not found.");

        StartCoroutine(PhaseOneLoop());
    }

    private IEnumerator PhaseOneLoop()
    {
        while (health.currentHP > health.maxHP * 0.66f)
        {
            if (currentState == State.Volley)
            {
                anim.SetTrigger("Charge1");
                yield return new WaitForSeconds(shotInterval);

                for (int i = 0; i < shotsPerVolley; i++)
                {
                    anim.SetTrigger("Attack");
                    SpawnAtPlayer(projectileBluePrefab, regularProjectileSpeed, regularProjectileDamage);
                    yield return new WaitForSeconds(shotInterval);
                }

                volleyCounter++;
                if (volleyCounter >= volleysBeforeAbility)
                {
                    volleyCounter = 0;
                    currentState = State.Ability;
                }
                else
                {
                    yield return new WaitForSeconds(volleyCooldown);
                }
            }
            else
            {
                anim.SetTrigger("Charge2");
                yield return new WaitForSeconds(shotInterval);

                for (int i = 0; i < circularShotCount; i++)
                {
                    float angle = i * 360f / circularShotCount;
                    SpawnDirectional(projectileBluePrefab, angle, abilityProjectileSpeed, abilityProjectileDamage);
                }

                currentState = State.Volley;
                yield return new WaitForSeconds(postAbilityCooldown);
            }

            yield return null;
        }
        // TODO: transition to Phase Two
    }

    private void SpawnAtPlayer(GameObject prefab, float speed, int damage)
    {
        if (player == null)
            return;

        Vector2 dir = (player.position - transform.position).normalized;
        var go = Instantiate(prefab, transform.position, Quaternion.identity);
        var proj = go.GetComponent<BlueProjectile>();
        if (proj != null)
        {
            proj.Init(dir, speed, damage);
        }
        else
        {
            Debug.LogError("SpawnAtPlayer: BlueProjectile component missing on prefab.");
        }
    }

    private void SpawnDirectional(GameObject prefab, float angleDeg, float speed, int damage)
    {
        Vector2 dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angleDeg), Mathf.Sin(Mathf.Deg2Rad * angleDeg));
        var go = Instantiate(prefab, transform.position, Quaternion.Euler(0, 0, angleDeg));
        var proj = go.GetComponent<BlueProjectile>();
        if (proj != null)
        {
            proj.Init(dir, speed, damage);
        }
        else
        {
            Debug.LogError("SpawnDirectional: BlueProjectile component missing on prefab.");
        }
    }
}