using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("The transform the camera will follow (e.g. the Player)")]
    public Transform target;

    [Tooltip("How quickly the camera catches up (higher = snappier)")]
    //public float smoothSpeed = 5f;
    public float smoothTime = 0.15f;

    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    private bool initialized = false;

    void Start()
    {
        // If the inspector target wasn’t set (or got cleared), find it by tag
        if (target == null)
        {
            var playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
                target = playerGO.transform;
            else
                Debug.LogError("[CameraFollow] No GameObject tagged 'Player' found!");
        }

        if (target != null)
            offset = transform.position - target.position;

        // Optional: listen for scene loads in case you need to re?find Player mid?game
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Awake()
    {
        // Calculate initial offset based on the current camera and target positions
        if (target != null)
            offset = transform.position - target.position;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += InitOnLoad;
        InitOnLoad(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= InitOnLoad;
    }

    private void InitOnLoad(Scene scene, LoadSceneMode mode)
    {
        // Find the player
        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO == null)
        {
            Debug.LogError($"[CameraFollow] Player not found in scene {scene.name}");
            initialized = false;
            return;
        }
        target = playerGO.transform;

        /*// Immediately position camera so the player is at the center
        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        // Now calculate offset for future smooth-following
        offset = transform.position - target.position;*/
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Scene has changed—grab the Player again and recompute offset
        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
        {
            target = playerGO.transform;
            offset = transform.position - target.position;
        }
    }

    /*void LateUpdate()
    {
        if (target == null) return;

        // Desired position is player position + our initial offset
        Vector3 desiredPos = target.position + offset;

        Vector3 smoothedPos = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            smoothTime
        );

        smoothedPos.z = transform.position.z;

        transform.position = smoothedPos;
    }*/

    void LateUpdate()
    {
        if (target == null) return;

        if (!initialized)
        {
            // snap the camera directly over the player, preserving Z
            transform.position = new Vector3(
                target.position.x,
                target.position.y,
                transform.position.z
            );
            // now compute the offset so smoothing starts from here
            offset = transform.position - target.position;
            velocity = Vector3.zero;
            initialized = true;
            return;  // skip smoothing this frame
        }

        // Smooth follow from here on out
        Vector3 desired = target.position + offset;
        Vector3 smoothed = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref velocity,
            smoothTime
        );
        smoothed.z = transform.position.z;
        transform.position = smoothed;
    }
}
