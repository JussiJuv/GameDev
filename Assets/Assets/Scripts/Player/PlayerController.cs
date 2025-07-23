using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Units per second")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 moveInput;

    // Dash state
    [Header("Dash Settings")]
    private bool isDashing;
    public float dashDistance = 5f;
    public float dashDuration = 0.05f;

    [Header("SFX")]
    public AudioClip dashSFX;
    public AudioClip drinkPotionSFX;

    public LayerMask wallMask;
    public LayerMask enemyMask;

    private AbilityData dashData;
    private AbilityManager abilityManager;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Health health;

    [HideInInspector] public bool isInvulnerable { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        health = GetComponent<Health>();
    }

    private void Start()
    {
        abilityManager = FindFirstObjectByType<AbilityManager>();
        if (abilityManager != null)
        {
            dashData = abilityManager.GetAbility("Dash");

            if (dashData == null)
            {
                Debug.Log("Dash ability not unlocked or not found on startup");
            }
            else
            {
                Debug.Log($"[PlayerController] Dash key = {dashData.activationKey}, cooldown = {dashData.cooldown}s");
            }

            // Subscribe to future unlocks
            abilityManager.OnAbilityUnlocked += OnAbilityUnlocked;
            // If dash is already unlocked at startup, grab it now
            TryCacheDash();
        }
    }

    private void OnAbilityUnlocked(AbilityData data)
    {
        if (data.abilityName == "Dash")
        {
            dashData = data;
            Debug.Log("[PlayerController] Dash unlocked via event!");
        }
    }

    private void TryCacheDash()
    {
        try
        {
            dashData = abilityManager.GetAbility("Dash");
        }
        catch
        {
            // Not unlocked yet
            dashData = null;
        }
    }

    private void Update()
    {
        // Read raw input axes
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        // Flip the sprite when moving left
        if (moveInput.x < 0f) spriteRenderer.flipX = true;
        else if (moveInput.x > 0f) spriteRenderer.flipX = false;

        // Detect dash key
        if (!isDashing 
            && dashData != null 
            && abilityManager.CanUse(dashData.abilityName) 
            && Input.GetKeyDown(dashData.activationKey))
        {
            Debug.Log("Dashing!");
            abilityManager.Consume(dashData.abilityName);
            StartCoroutine(PerformDash());
        }

        // Healing
        if (Input.GetKeyDown(KeyCode.F))
        {
            float fraction;
            if (PlayerInventory.Instance.ConsumeActive(out fraction))
            {
                AudioManager.Instance.PlaySFX(drinkPotionSFX);
                int amount = Mathf.CeilToInt(fraction * health.maxHP);
                health.Heal(amount);
            }
        }
    }

    private void FixedUpdate()
    {
        // If we are currently dashing we skip normal movement
        if (isDashing) return;

        // Regular walk
        Vector2 newPos = rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    private IEnumerator PerformDash()
    {
        // Compute target point
        Vector2 direction = moveInput != Vector2.zero
            ? moveInput.normalized
            : Vector2.right;
        Vector2 origin = rb.position;
        Vector2 desired = origin + direction * dashDistance;

        // Raycast to walls so we dont phase through them
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, dashDistance, wallMask);
        Vector2 target = hit.collider != null
            ? hit.point - direction * 0.1f
            : desired;

        // Temporarily ignore enemies & make invulnerable
        isDashing = true;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMaskToLayer(enemyMask), true);

        AudioManager.Instance.PlaySFX(dashSFX);

        // "Teleport" instantly
        rb.position = target;

        // small delay so dash feels visible
        yield return new WaitForSeconds(dashDuration);

        // Re-enable collisions & vulnerability
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMaskToLayer(enemyMask), false);
        isDashing = false;
    }

    private int LayerMaskToLayer(LayerMask mask)
    {
        int m = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((m & (1 << i)) != 0) return i;
        }
        return 0;
    }
}
