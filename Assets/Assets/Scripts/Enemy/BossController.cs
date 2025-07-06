using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Health))]
public class BossController : MonoBehaviour
{
    [Header("Activation")]
    [Tooltip("Only chase when true")]
    public bool canActivate = false;

    [Header("Split Settings")]
    [Tooltip("Prefab for the small slimes to spawn on death")]
    public GameObject smallSlimePrefab;
    public int splitCount = 4;
    public float spawnRadius = 1f;

    [Header("Movement & Targeting")]
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
    public int attackDamage = 1;
    public float attackCooldown = 1f;

    [Header("Key Reward")]
    [Tooltip("The key the boss drops on death")]
    public KeyItemData bossKeyData;

    [Header("UI")]
    private BossHealthBarUI bossHealthBarUI;
    public string bossName;

    private Transform playerT;
    private float lastAttackTime = 0f;
    private Health health;

    public BossRoomEntrance gateController;

    /*private void Start()
    {
        bossHealthBarUI = FindFirstObjectByType<BossHealthBarUI>();
        if (bossHealthBarUI == null) Debug.LogError("[BossController]: BossHealthBarUI not found in loaded scenes");
    }*/

    private void Awake()
    {
        health = GetComponent<Health>();
        // Call OnBossDeath when Health reaches zero
        health.OnDeath.AddListener(OnBossDeath);

        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerT = player.transform;
        }
        else
        {
            Debug.LogError("BossController: Player tag not found in scene.");
        }
    }
    void Update()
    {
        if (!canActivate || playerT == null || health.currentHP <= 0)
            return;

        // Movement
        Vector3 toPlayer = playerT.position - transform.position;
        float dist = toPlayer.magnitude;
        Vector3 dir = toPlayer.normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Attack
        if (dist <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        var ph = playerT.GetComponent<Health>();
        if (ph != null) ph.TakeDamage(attackDamage);
    }


    public void ActivateBoss()
    {
        if (bossHealthBarUI == null)
        {
            // Grab the UI scene
            var uiScene = SceneManager.GetSceneByName("UI");
            if (!uiScene.isLoaded)
            {
                Debug.LogError("UI scene not loaded");
                return;
            }

            // Look through its root GameObjects
            foreach (var root in uiScene.GetRootGameObjects())
            {
                // Find the canvas
                var canvas = root.GetComponentInChildren<Canvas>(true);
                if (canvas == null) continue;

                var panelTf = canvas.transform.Find("BossHealthBarPanel");
                if (panelTf != null)
                {
                    bossHealthBarUI = panelTf.GetComponent<BossHealthBarUI>();
                    break;
                }
            }

            if (bossHealthBarUI == null)
            {
                Debug.LogError("[BossController]: Could not find BossHealthBarUI in UI scene");
                return;
            }

            /*bossHealthBarUI = FindFirstObjectByType<BossHealthBarUI>();
            if (bossHealthBarUI == null)
            {
                Debug.LogError("[BossController]: BossHealthBarUI still missing during ActivateBoss");
                return;
            }*/
        }

        canActivate = true;
        // Show the boss hp bar
        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.Show(health, bossName);
        }
        else
        {
            Debug.LogWarning("[BossController]: bossHealthBarUI is null");
        }
    }

    /// <summary>
    /// Called Via Health.OnDeath when the boss actually dies.
    /// </summary>
    public void OnBossDeath()
    {
        // Give key directly into inventory
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var inv = player.GetComponent<PlayerInventory>();
            if (inv != null && bossKeyData != null) 
            {
                inv.AddKey(bossKeyData);
                inv.DebugLogInventory();
            }
        }

        // Show pickup popup
        KeyPickupPopupUI.Instance.Show(bossKeyData);


        // Hide the boss hp bar
        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.Hide();
        }
        else
        {
            Debug.LogWarning("[BossController]: bossHealthBarUI is null");
        }

        // Spawn small slimes, award key, open the 
        if (smallSlimePrefab != null)
        {
            for (int i = 0; i < splitCount; i++)
            {
                float angle = i * Mathf.PI * 2f / splitCount;
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnRadius;
                Instantiate(smallSlimePrefab, transform.position + offset, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("BossController: smallSlimePrefab not assigned!");
        }

        gateController.ReleaseGate();
    }

    // Draw the attack range in-editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
