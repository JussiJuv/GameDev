using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GrayscaleTint : MonoBehaviour
{
    [Range(0f, 1f)]
    public float grayAmount = 0.5f; // 0 = full color, 1 = full gray

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ApplyGray();
    }

#if UNITY_EDITOR
    // So the tint updates live when adjusting in inspector
    void OnValidate()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        ApplyGray();
    }
#endif

    void ApplyGray()
    {
        float r = Mathf.Lerp(1f, 0.5f, grayAmount);
        float g = Mathf.Lerp(1f, 0.5f, grayAmount);
        float b = Mathf.Lerp(1f, 0.5f, grayAmount);
        sr.color = new Color(r, g, b, 1f); // Keep alpha = 1
    }
}
