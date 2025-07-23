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

    [Header("SFX")]
    public AudioClip arrowRainHitSFX;
    public AudioClip arrowRainMissSFX;

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

    private Collider2D[] DoArrowRainAOE(Vector3 center)
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
        return hits;
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
        Collider2D[] hits = DoArrowRainAOE(center);

        if (hits.Length > 0)
            AudioManager.Instance.PlaySFX(arrowRainHitSFX);
        else
            AudioManager.Instance.PlaySFX(arrowRainMissSFX);

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