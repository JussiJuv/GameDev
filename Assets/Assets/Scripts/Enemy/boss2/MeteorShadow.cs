using UnityEngine;

public class MeteorShadow : MonoBehaviour
{
    [Tooltip("Time from spawn to impact")]
    public float fallTime = 1.0f;
    [Tooltip("Final scale of the shadow when meteor hits")]
    public Vector3 maxScale = Vector3.one;
    private SpriteRenderer sr;
    private float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.zero;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / fallTime);
        // scale from zero ? maxScale
        transform.localScale = Vector3.Lerp(Vector3.zero, maxScale, t);
        // alpha from 0.2 ? 1.0
        Color c = sr.color;
        c.a = Mathf.Lerp(0.2f, 1.0f, t);
        sr.color = c;
        // at end of fallTime, destroy shadow
        if (timer >= fallTime) Destroy(gameObject);
    }
}
