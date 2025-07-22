using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class MirrorBinderController : MonoBehaviour
{
    private List<GameObject> activeClones = new List<GameObject>();

    [Header("Dev Settings")]
    public bool devStartPhase2 = false;
    public bool isClone = false;

    [Header("UI")]
    private BossHealthBarUI bossHealthBarUI;
    public string bossName;

    [Header("Gate & Entrance")]
    public GameObject fogGate;
    public BossGateController gateController;

    [Header("Boss Home Point")]
    public Transform homePoint;
    public float moveSpeed = 5f;

    // Phase HP thresholds
    private float phaseOneToTwoHP;
    private float phaseTwoToThreeHP;

    [Header("Meteor Settings")]
    public bool enableMeteors = true;
    private float currentMeteorInterval;
    public GameObject meteorPrefab;
    public float p1DropDuration = 1.0f;
    public float p2DropDuration = 0.8f;
    public float p3DropDuration = 0.6f;

    [Header("Phase One (100%-66%)")]
    [Min(1)] public int p1VolleysBeforeSpecial = 3;
    [Min(1)] public int p1ShotsPerVolley = 3;
    [Min(0)] public float p1ShotInterval = 0.5f;
    [Min(0)] public float p1VolleyCooldown = 1.5f;
    public float p1MeteorInterval = 8f;

    [Header("Phase One Ability: Circular Blast")]
    [Min(1)] public int circularShotCount = 12;
    [Min(0)] public float circularSpeed = 2f;
    public int circularDamage = 1;
    [Min(0)] public float p1PostAbilityCooldown = 2f;

    [Header("Phase Two (66%-33%)")]
    [Min(1)] public int p2ClonesCount = 6;
    [Min(0)] public float p2CloneRadius = 6f;
    [Min(0)] public float p2CloneChargeTime = 2f;
    [Min(1)] public int p2VolleysBeforeSpecial = 3;
    [Min(1)] public int p2ShotsPerVolley = 3;
    [Min(0)] public float p2ShotInterval = 0.5f;
    [Min(0)] public float p2VolleyCooldown = 1.5f;
    public float p2MeteorInterval = 6f;
    public GameObject clonePrefab;

    [Header("Phase Two Red Projectile")]
    public GameObject redProjectilePrefab;
    [Min(0)] public float redProjectileSpeed = 4f;
    public int redProjectileDamage = 1;

    [Header("Phase Three (33%-0%)")]
    [Min(1)] public int p3ClonesCount = 8;
    [Min(0)] public float p3CloneRadius = 8f;
    [Min(0)] public float p3CloneChargeTime = 1.5f;
    [Min(1)] public int p3VolleysBeforeSpecial = 3;
    [Min(1)] public int p3ShotsPerVolley = 5;
    [Min(0)] public float p3ShotInterval = 0.4f;
    [Min(0)] public float p3VolleyCooldown = 1.0f;
    public float p3MeteorInterval = 4f;

    [Header("Projectile Settings")]
    [Min(0)] public float regularProjectileSpeed = 5f;
    public int regularProjectileDamage = 1;
    public GameObject projectileBluePrefab;

    [Header("Rewards")]
    public KeyItemData goldKeyData;
    public KeyItemData shardBData;

    private enum State { Volley, Special }
    private State currentState;
    private int volleyCounter;
    private int specialCounter;

    private Health health;
    private Animator anim;
    private Transform player;
    private Vector3 originalPosition;

    private void Awake()
    {
        health = GetComponent<Health>();
        anim = GetComponent<Animator>();
        originalPosition = transform.position;

        phaseOneToTwoHP = health.maxHP * 0.66f;
        phaseTwoToThreeHP = health.maxHP * 0.33f;

        // Prevent Health.Die() from disabling Animator and this controller
        try
        {
            var healthType = health.GetType();
            var field = healthType.GetField("disableOnDeath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                var arr = (Behaviour[])field.GetValue(health);
                var list = new List<Behaviour>(arr);
                list.Remove(anim);
                list.Remove(this);
                field.SetValue(health, list.ToArray());
            }
        }
        catch { }
        {
            health = GetComponent<Health>();
            anim = GetComponent<Animator>();
            originalPosition = transform.position;

            phaseOneToTwoHP = health.maxHP * 0.66f;
            phaseTwoToThreeHP = health.maxHP * 0.33f;
        }

        if (!isClone) health.OnDeath.AddListener(CleanUpAllClones);
        if (bossHealthBarUI != null) bossHealthBarUI.Hide();

        // Find gateController if not already known
        if (gateController == null)
        {
            gateController = FindFirstObjectByType<BossGateController>();
            if (gateController == null)
                Debug.LogError("[MirrorBinderController]: No BossGateController found");
        }
    }

    private void Start()
    {
        if (isClone) return;
        if (homePoint == null) homePoint = new GameObject("BossHomePoint").transform;
        homePoint.position = originalPosition;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("Player tag not found.");

       /* if (devStartPhase2 && health.currentHP <= phaseOneToTwoHP)
            StartCoroutine(PhaseTwoLoop());
        else if (health.currentHP > phaseOneToTwoHP)
            StartCoroutine(PhaseOneLoop());
        else if (health.currentHP > phaseTwoToThreeHP)
            StartCoroutine(PhaseTwoLoop());
        else
            StartCoroutine(PhaseThreeLoop());

        // Launch meteor routine
        if (enableMeteors)
            StartCoroutine(MeteorRoutine());

        ActivateBoss();*/
    }

    private IEnumerator MeteorRoutine()
    {
        // choose the right interval for the starting phase
        currentMeteorInterval = (health.currentHP > phaseOneToTwoHP)
            ? p1MeteorInterval
            : (health.currentHP > phaseTwoToThreeHP)
                ? p2MeteorInterval
                : p3MeteorInterval;

        while (health.currentHP > 0)
        {
            yield return new WaitForSeconds(currentMeteorInterval);
            SpawnMeteor();

            // if we passed a threshold, update the interval
            if (health.currentHP <= phaseTwoToThreeHP)
                currentMeteorInterval = p3MeteorInterval;
            else if (health.currentHP <= phaseOneToTwoHP)
                currentMeteorInterval = p2MeteorInterval;
        }
    }

    private IEnumerator PhaseOneLoop()
    {
        currentState = State.Volley;
        volleyCounter = 0;

        while (health.currentHP > phaseOneToTwoHP)
        {
            if (currentState == State.Volley)
            {
                yield return DoVolley(p1ShotsPerVolley, regularProjectileSpeed, regularProjectileDamage, p1ShotInterval);
                volleyCounter++;
                if (volleyCounter >= p1VolleysBeforeSpecial)
                {
                    volleyCounter = 0;
                    currentState = State.Special;
                }
                else yield return new WaitForSeconds(p1VolleyCooldown);
            }
            else
            {
                yield return DoCircularBlast();
                currentState = State.Volley;
                yield return new WaitForSeconds(p1PostAbilityCooldown);
            }
        }
        StartCoroutine(PhaseTwoLoop());
    }

    private IEnumerator PhaseTwoLoop()
    {
        currentState = State.Volley;
        volleyCounter = specialCounter = 0;

        while (health.currentHP > phaseTwoToThreeHP)
        {
            if (currentState == State.Volley)
            {
                yield return DoVolley(p2ShotsPerVolley, regularProjectileSpeed, regularProjectileDamage, p2ShotInterval);
                volleyCounter++;
                if (volleyCounter >= p2VolleysBeforeSpecial)
                {
                    volleyCounter = 0;
                    currentState = State.Special;
                }
                else yield return new WaitForSeconds(p2VolleyCooldown);
            }
            else
            {
                if (specialCounter % 2 == 0)
                    yield return DoClonePhase(p2ClonesCount, p2CloneRadius, p2CloneChargeTime);
                else
                    yield return DoCircularBlast();

                specialCounter++;
                currentState = State.Volley;
            }
        }
        StartCoroutine(PhaseThreeLoop());
    }

    private IEnumerator PhaseThreeLoop()
    {
        int circularCount = 0;
        while (health.currentHP > 0)
        {
            // 1) Volley
            yield return DoVolley(p3ShotsPerVolley, regularProjectileSpeed, regularProjectileDamage, p3ShotInterval);
            // 2) Circular immediately
            yield return DoCircularBlast();
            circularCount++;
            // 3) Clone after 3 blasts
            if (circularCount >= 3)
            {
                // Do one more volley attack
                yield return DoVolley(p3ShotsPerVolley, regularProjectileSpeed, regularProjectileDamage, p3ShotInterval);
                circularCount = 0;
                yield return DoClonePhase(p3ClonesCount, p3CloneRadius, p3CloneChargeTime);
                // Move back
                anim.SetTrigger("Move");
                yield return MoveTo(homePoint.position);
            }
            // 4) Wait
            yield return new WaitForSeconds(p3VolleyCooldown);
        }
    }

    private void SpawnMeteor()
    {
        Debug.Log("[MirrorBinder]: Firing meteor");
        if (player == null || meteorPrefab == null) return;

        float duration =
            health.currentHP > phaseOneToTwoHP ? p1DropDuration :
            health.currentHP > phaseTwoToThreeHP ? p2DropDuration :
            p3DropDuration;

        Vector3 dropPos = player.position;
        var go = Instantiate(meteorPrefab);
        var fall = go.GetComponent<MeteorFall>();
        fall.dropDuration = duration;
        fall.damage = redProjectileDamage;
        fall.Init(dropPos, player);
    }

    private IEnumerator DoVolley(int shots, float speed, int damage, float interval)
    {
        anim.SetTrigger("Charge1");
        yield return new WaitForSeconds(interval);
        for (int i = 0; i < shots; i++)
        {
            anim.SetTrigger("Attack");
            SpawnAtPlayer(projectileBluePrefab, speed, damage);
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator DoCircularBlast()
    {
        anim.SetTrigger("Charge2");
        yield return new WaitForSeconds(p1ShotInterval);
        for (int i = 0; i < circularShotCount; i++)
        {
            float angle = i * 360f / circularShotCount;
            SpawnDirectional(projectileBluePrefab, angle, circularSpeed, circularDamage);
        }
    }

    private IEnumerator DoClonePhase(int cloneCount, float radius, float chargeTime)
    {
        // Start the telegraph animation
        anim.SetTrigger("Charge2");
        yield return new WaitForSeconds(p1ShotInterval);

        // Snap player and calculate positions
        if (player != null) player.position = originalPosition;
        int total = cloneCount + 1;
        int bossIndex = Random.Range(0, total);
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < total; i++)
            positions.Add(originalPosition + Quaternion.Euler(0, 0, i * 360f / total) * Vector3.up * radius);

        // Teleport boss and spawn clones
        transform.position = positions[bossIndex];
        //var clones = new List<GameObject>();
        for (int i = 0; i < total; i++)
        {
            if (i == bossIndex) continue;
            var go = Instantiate(clonePrefab, positions[i], Quaternion.identity);
            go.GetComponent<MirrorBinderController>().isClone = true;
            //clones.Add(go);
            activeClones.Add(go);
        }

        // Charge timer, able to be interrupted only by hitting the boss
        float startHP = health.currentHP;
        float timer = chargeTime;
        bool interrupted = false;

        while (timer > 0)
        {
            // If boss was hit, play hit animation, brief pause, then cancel
            if (health.currentHP < startHP)
            {
                interrupted = true;
                anim.SetTrigger("Hit");
                yield return new WaitForSeconds(0.2f);  // allow the hit anim to show
                break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }

        // If not interrupted, fire homing red bolts
        if (!interrupted)
        {
            SpawnHoming(transform.position);
            foreach (var c in activeClones)
            {
                if (c == null) continue;
                SpawnHoming(c.transform.position);
            }
                
        }

        // Clean up all clones
        CleanUpAllClones();
        /*foreach (var c in activeClones)
            Destroy(c);*/

        // Move back to center
        anim.SetTrigger("Move");
        yield return StartCoroutine(MoveTo(homePoint.position));
    }

    private void CleanUpAllClones()
    {
        foreach (var c in activeClones)
            if (c != null) Destroy(c);
        activeClones.Clear();
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while ((transform.position - target).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }

    private void SpawnAtPlayer(GameObject prefab, float speed, int damage)
    {
        if (player == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        var go = Instantiate(prefab, transform.position, Quaternion.identity);
        var proj = go.GetComponent<BlueProjectile>();
        if (proj != null) proj.Init(dir, speed, damage);
    }

    private void SpawnDirectional(GameObject prefab, float angle, float speed, int damage)
    {
        Vector2 dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
        var go = Instantiate(prefab, transform.position, Quaternion.Euler(0, 0, angle));
        var proj = go.GetComponent<BlueProjectile>();
        if (proj != null) proj.Init(dir, speed, damage);
    }

    private void SpawnHoming(Vector3 from)
    {
        var go = Instantiate(redProjectilePrefab, from, Quaternion.identity);
        var rp = go.GetComponent<RedProjectile>();
        if (rp != null) rp.InitStraight(player, redProjectileSpeed, redProjectileDamage);
    }

    // Called (UnityEvent) when the gate locks and fight should begin
    public void ActivateBoss()
    {
        // Lock the gate so the player cant leave
        gateController?.LockGate();

        if (bossHealthBarUI == null)
        {
            bossHealthBarUI = FindFirstObjectByType<BossHealthBarUI>();
            if (bossHealthBarUI == null)
            {
                Debug.LogError("[MirrorbinderController]: Could not find BossHealthBarUI");
                return;
            }
            /*// Grab the already?loaded UI scene
            var uiScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName("UI");
            if (!uiScene.isLoaded)
            {
                Debug.LogError("UI scene not loaded");
                return;
            }

            // Find the BossHealthBarUI component under BossHealthBarPanel
            foreach (var root in uiScene.GetRootGameObjects())
            {
                var panel = root.GetComponentInChildren<Transform>(true)
                                ?.Find("BossHealthBarPanel");
                if (panel != null)
                {
                    bossHealthBarUI = panel.GetComponent<BossHealthBarUI>();
                    break;
                }
            }

            if (bossHealthBarUI == null)
                Debug.LogError("[MirrorBinderController]: Could not find BossHealthBarUI");*/
        }

        // Show it
        bossHealthBarUI.Show(health, bossName);

        if (!isClone)
        {
            // Starting phase
            if (devStartPhase2 && health.currentHP <= phaseOneToTwoHP)
                StartCoroutine(PhaseTwoLoop());
            else if (health.currentHP > phaseOneToTwoHP)
                StartCoroutine(PhaseOneLoop());
            else if (health.currentHP > phaseTwoToThreeHP)
                StartCoroutine(PhaseTwoLoop());
            else
                StartCoroutine(PhaseThreeLoop());

            // Meteor
            if (enableMeteors)
                StartCoroutine(MeteorRoutine());
        }
    }

    /// <summary>
    /// Invoked by Health.OnDeath when the boss dies
    /// </summary>
    public void OnBossDeath()
    {
        // Give rewards to player
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var inv = player.GetComponent<PlayerInventory>();
            if (inv != null)
            {
                if (goldKeyData != null) inv.AddKey(goldKeyData);
                if (shardBData != null) inv.AddKey(shardBData);
            }
        }

        PickupPopuiUI.Instance.Show(new[] { goldKeyData, shardBData });

        bossHealthBarUI?.Hide();

        // Unlock gate
        gateController?.UnlockGate();

        // Disable fog gate
        fogGate?.SetActive(false);

        CleanUpAllClones();
    }
}