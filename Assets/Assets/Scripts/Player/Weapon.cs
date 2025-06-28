using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public Transform bowTransform;

    [Header("Bow Settings")]
    [Tooltip("Distance from the player center to the bow")]
    public float bowRadius = 0.5f;

    [Tooltip("")]
    public float bowDeadZoneRadius = 0.9f;

    [Header("Projectile Settings")]
    [Tooltip("Prefab of the projectile to fire")]
    public GameObject projectilePrefab;

    /*[Tooltip("Where on the player the projectile spawns")]
    public Transform firePoint;*/

    [Tooltip("Shots per second")]
    public float fireRate = 1f;

    [Tooltip("Initial speed of the projectile")]
    public float projectileSpeed = 10f;

    private float nextFireTime = 0f;
    private Vector2 lastAimDirection = Vector2.right;

    private Camera mainCam;

    [Header("Arrow Rain Blocker")]
    public ArrowRainAbility ArrowRainAbility;

    private void Awake()
    {
        mainCam = Camera.main;
        ArrowRainAbility = GetComponent<ArrowRainAbility>();
    }

    private void Update()
    {
        AimAtMouse();

        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            // If we are in arrow rain targeting mode, dont shoot
            if (ArrowRainAbility != null && ArrowRainAbility.IsTargeting)
            {
                Debug.Log("Weapon Blocked normal shot during arrow rain");
            }
            else
            {
                Shoot();
                nextFireTime = Time.time +1f / fireRate;
            }
        }

        /*// If arrow rain targeting is active, skip normal shooting
        if (ArrowRainAbility != null && ArrowRainAbility.IsTargeting)
        {
            return;
        }

        // Fire when the Fire1 button (left CTRL / mouse0 / configured) is pressed
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }*/
    }

    void AimAtMouse()
    {
        // Get mouse position in world space
        Vector3 mouseScreen = Input.mousePosition;
        Vector3 mouseWorld = mainCam.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector3 playerPos = transform.position;
        //Vector2 rawDir = mouseWorld - firePoint.position;
        Vector2 rawDir = mouseWorld - playerPos;
        if (rawDir.sqrMagnitude < bowDeadZoneRadius) return;

        lastAimDirection = (mouseWorld - firePoint.position).normalized;

        float angle = Mathf.Atan2(lastAimDirection.y, lastAimDirection.x) * Mathf.Rad2Deg;
        angle += 180f;  // Correct bow orientation.

        // Apply rotation
        var q = Quaternion.Euler(0, 0, angle);
        firePoint.rotation = q;
        bowTransform.rotation = q;

        // Reposition the bow in a circle around the player
        //Vector3 playerPos = transform.position;
        Vector3 bowOffset = (Vector3)lastAimDirection * bowRadius;
        bowTransform.position = playerPos + bowOffset;

        firePoint.position = bowTransform.position;
    }

    void Shoot()
    {

        // Compute the raw firing angle
        float rawAngle = Mathf.Atan2(lastAimDirection.y, lastAimDirection.x) * Mathf.Rad2Deg;
        // Rotate the arrow
        Quaternion arrowRot = Quaternion.Euler(0, 0, rawAngle - 90);

        // Instantiate with the same rotation as firePoint
        var proj = Instantiate(projectilePrefab, firePoint.position, arrowRot);

        //Rigidbody2D rb = projectilePrefab.GetComponent<Rigidbody2D>();
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = lastAimDirection * projectileSpeed;
        }
        else
        {
            Debug.LogWarning("Projectile prefab has no RigidBody2D!");
        }

        // Debug: draw the fire direction in the Scene view
        Debug.DrawRay(firePoint.position, lastAimDirection, Color.red, 1f);
    }
}
