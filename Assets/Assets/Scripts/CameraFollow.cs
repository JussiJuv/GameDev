using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("The transform the camera will follow (e.g. the Player)")]
    public Transform target;

    [Tooltip("How quickly the camera catches up (higher = snappier)")]
    //public float smoothSpeed = 5f;
    public float smoothTime = 0.15f;

    private Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        // Calculate initial offset based on the current camera and target positions
        if (target != null)
            offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position is player position + our initial offset
        Vector3 desiredPos = target.position + offset;
        // Smoothly interpolate from current camera pos to desired pos
        //Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        // Preserve original Z (in case Lerp nudged it)

        Vector3 smoothedPos = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            smoothTime
        );

        smoothedPos.z = transform.position.z;

        transform.position = smoothedPos;
    }
}
