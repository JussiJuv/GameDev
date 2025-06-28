using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 mouse = Input.mousePosition;
        Vector3 world = _cam.ScreenToWorldPoint(mouse);
        world.z = 0f;
        transform.position = world;
    }
}
