using UnityEngine;

public class SurfaceSensor : MonoBehaviour
{
    public string CurrentSurfaceTag { get; private set; }

    private Collider2D currentSurfaceCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // only consider ground layer
        if (((1 << collision.gameObject.layer) & PlayerFootstep.GroundMask) == 0) return;

        Debug.Log($"[SurfaceSensor]: Entered '{collision.name}' tag={collision.tag} layer={LayerMask.LayerToName(collision.gameObject.layer)}");
        currentSurfaceCollider = collision;
        CurrentSurfaceTag = collision.tag;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == currentSurfaceCollider)
        {
            Debug.Log($"[SurfaceSensor] Exited '{collision.name}'");
            currentSurfaceCollider = null;
            CurrentSurfaceTag = null;
        }
    }
}
