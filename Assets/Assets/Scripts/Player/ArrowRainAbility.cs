using UnityEngine;
using System.Collections;

public class ArrowRainAbility : MonoBehaviour
{
    [Header("Inspector Assign")]
    public AbilityManager abilityManager;
    public GameObject indicatorPrefab;
    public string abilityName = "Arrow Rain";

    private AbilityData _data;
    private GameObject _indicator;
    private bool _isTargeting;

    [Header("Visual Rain Settings")]
    [Tooltip("Visual sprite with no collider")]
    public GameObject arrowVisualPrefab;
    public float visualFallSpeed = 30f;
    public float stickDuration = 1f;
    public Transform arrowContainer;

    /// <summary>
    /// Is the circle up right now? PlayerController can check this
    /// to block normal shooting.
    /// </summary>
    public bool IsTargeting => _isTargeting;

    void Update()
    {
        // 1) Start targeting
        if (!_isTargeting
            && Input.GetKeyDown(KeyCode.Alpha1)
            && abilityManager.CanUse(abilityName))
        {
            _data = abilityManager.GetAbility(abilityName);
            //abilityManager.Consume(abilityName);
            BeginTargeting();
        }

        // 2) Confirm with left click
        if (_isTargeting && Input.GetMouseButtonDown(0))
        {
            Vector3 center = _indicator.transform.position;
            EndTargeting();
            StartCoroutine(SpawnArrowRainVisual(center));
            abilityManager.Consume(abilityName);
        }

        // 3) Cancel with right click or Esc
        if (_isTargeting
            && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
        {
            EndTargeting();
        }
    }

    private void BeginTargeting()
    {
        _isTargeting = true;
        _indicator = Instantiate(indicatorPrefab);
        _indicator.transform.localScale = Vector3.one * _data.areaRadius * 2f;
    }

    private void EndTargeting()
    {
        _isTargeting = false;
        if (_indicator != null) Destroy(_indicator);
    }

    private void DoArrowRainAOE(Vector3 center)
    {
        // purely 2D overlap circle
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            _data.areaRadius,
            LayerMask.GetMask("Enemy")
        );

        Debug.Log($"[ArrowRainAbility] AOE at {center}, found {hits.Length} enemies");

        foreach (var c in hits)
        {
            var h = c.GetComponent<Health>();
            if (h != null)
            {
                h.TakeDamage(_data.arrowDamage);
                Debug.Log($" - Damaged {c.name} for {_data.arrowDamage}");
            }
        }

    }

    private IEnumerator SpawnArrowRainVisual(Vector3 center)
    {
        // Precompute random offsets
        var offsets = new Vector2[_data.arrowCount];
        for (int i = 0; i < offsets.Length; i++)
            offsets[i] = Random.insideUnitCircle * _data.areaRadius;

        // Spawn visuals at height
        var arrows = new GameObject[offsets.Length];
        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3 spawnPos = center + (Vector3)offsets[i]
                             + Vector3.up * (_data.dropHeight);
            arrows[i] = Instantiate(
                arrowVisualPrefab,
                spawnPos,
                Quaternion.Euler(0,0,180f),
                arrowContainer
            );
        }

        // Move them down over time
        float remainingHeight = _data.dropHeight;
        while (remainingHeight > 0f)
        {
            float step = visualFallSpeed * Time.deltaTime;
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].transform.position += Vector3.down * step;
            }
            remainingHeight -= step;
            yield return null;
        }

        // Snap to exact circle?scatter positions, then destroy
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].transform.position = center + (Vector3)offsets[i];
        }

        // Apply damage to everything in that circle
        DoArrowRainAOE(center);

        // Let the arrows stick around for a bit
        yield return new WaitForSeconds(stickDuration);

        // Destroy visuals
        int count = _data.arrowCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(arrows[i]);
        }
    }


    // debug: draw the circle in Scene view
    void OnDrawGizmosSelected()
    {
        if (_isTargeting && _indicator != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(
                _indicator.transform.position,
                _data != null ? _data.areaRadius : 1f
            );
        }
    }
}


/*using UnityEngine;
using System.Collections;

public class ArrowRainAbility : MonoBehaviour
{
    [Header("References")]
    public AbilityManager abilityManager;
    public GameObject indicatorPrefab;

    [Header("Tunable Settings")]
    public string abilityName = "Arrow Rain";
    public float indicatorRadius = 3f;
    public float dropHeight = 10f;
    public float fallSpeed = 20f;
    public LayerMask groundMask;
    public Transform arrowContainer;    // Parent for spawned arrows

    private AbilityData _data;
    private GameObject _indicator;
    private bool _isTargeting;

    private void Start()
    {
        if (abilityManager == null)
        {
            abilityManager = FindFirstObjectByType<AbilityManager>();
        }
        _data = abilityManager.GetAbility(abilityName);
    }

    private void Update()
    {
        // Activate targeting on key "1"
        if (!_isTargeting
            && Input.GetKeyDown(KeyCode.Alpha1)
            && abilityManager.CanUse(abilityName))
        {
            BeginTargeting();
        }

        // Confirm with left-click
        if (_isTargeting && Input.GetMouseButtonDown(0))
        {
            Vector3 targetPos = _indicator.transform.position;
            abilityManager.Consume(abilityName);
            EndTargeting();
            StartCoroutine(SpawnArrowRain(targetPos));
        }

        // Cancel Targeting with right-click or Esc
        else if (_isTargeting && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))) 
        {
            EndTargeting();
        }
    }

    private void BeginTargeting()
    {
        _isTargeting = true;
        _indicator = Instantiate(indicatorPrefab);
        _indicator.transform.localScale = Vector3.one * indicatorRadius * 2f;
    }

    private void EndTargeting()
    {
        _isTargeting = false;
        if (_indicator != null) Destroy(_indicator);
    }

    private IEnumerator SpawnArrowRain(Vector3 center)
    {
        for (int i = 0; i < _data.arrowCount; i++)
        {
            // Random point inside circle
            Vector2 offset = Random.insideUnitCircle * indicatorRadius;
            Vector3 spawnPos = center + (Vector3)offset + Vector3.up * dropHeight;

            // instantiate arrow prefab
            GameObject arrow = Instantiate(_data.arrowPrefab, spawnPos, Quaternion.identity, arrowContainer);

            // Give it a downward velocity
            var rb = arrow.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.down * fallSpeed;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
}
*/