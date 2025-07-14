using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(Health))]
public class SlimeBossArenaController : MonoBehaviour
{
    [Header("Movement & Attack")]
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public int attackDamage = 1;
    public float attackCooldown = 1f;

    [Header("Split Settings")]
    public GameObject smallSlimePrefab;
    public int splitCount = 4;
    public float spawnRadius = 1f;

    [Header("Spawn Delay")]
    [Tooltip("Seconds to wait before boss starts moving")]
    public float spawnDelay = 1f;

    bool isActive = false;

    [Header("Arena Gate")]
    public StoneGateController gateController;

    [Header("UI")]
    public string bossName = "Slime Giant";

    Health health;
    Transform player;
    BossHealthBarUI hpBarUI;
    float lastAttackTime;

    private void Awake()
    {
        health = GetComponent<Health>();
        health.OnDeath.AddListener(HandleDeath);

        var pObj = GameObject.FindWithTag("Player");
        if (pObj != null)
            player = pObj.transform;
        else
            Debug.LogError("[SlimeBossArenaController]: No Player tagged object in scene");

        // Find the gateController in the scene
        if (gateController == null)
        {
            gateController = FindFirstObjectByType<StoneGateController>();
            if (gateController == null)
                Debug.LogError("[SlimeBossArenaController]: No StoneGateController in scene");
        }

        // Find and show boss bar
        /*var uiScene = SceneManager.GetSceneByName("UI");
        if (!uiScene.isLoaded)
            Debug.LogError("[SlimeBossArenaController]: UI scene not loaded");
        else
        {
            foreach (var root in uiScene.GetRootGameObjects())
            {
                var bar = root.GetComponentInChildren<BossHealthBarUI>(true);
                if (bar != null)
                {
                    hpBarUI = bar;
                    break;
                }
            }
            if (hpBarUI == null)
                Debug.LogError("[SlimeBossArenaController]: Couuld not find BossHealthBarUI");
            else
                hpBarUI.Show(health, bossName);
        }*/

        hpBarUI = FindFirstObjectByType<BossHealthBarUI>();
        if (hpBarUI != null)
            hpBarUI.Show(health, bossName);
        else
            Debug.LogError("[SlimeBossArenaController]: Could not find BossHealthBarUI");

        // delay boss activation
        StartCoroutine(SpawnThenActivate());
    }

    private void Update()
    {
        if (!isActive || player == null || health.currentHP <= 0) return;

        float dist = Vector2.Distance(transform.position, player.position); ;
        if (dist <= attackRange)
            TryAttack();
        else
            ChasePlayer();
    }

    private IEnumerator SpawnThenActivate()
    {
        yield return new WaitForSeconds(spawnDelay);
        isActive = true;
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
            );
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;
        var ph = player.GetComponent<Health>();
        if (ph != null) ph.TakeDamage(attackDamage);
    }

    void HandleDeath()
    {
        // spawn small slimes
        if (smallSlimePrefab != null)
        {
            for (int i = 0; i < splitCount; i++)
            {
                float angle = i * Mathf.PI * 2f / splitCount;
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnRadius;
                Instantiate(smallSlimePrefab, transform.position + offset, Quaternion.identity);
            }
        }

        // Hide boss bar
        if (hpBarUI != null)
            hpBarUI.Hide();

        // Open the gate
        if (gateController != null)
            gateController.OpenGate();
    }
}
