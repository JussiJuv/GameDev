using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    [Header("Surface Clips")]
    public AudioClip[] stoneSteps;
    public AudioClip[] grassSteps;
    public AudioClip[] woodSteps;

    [Header("Step Settings")]
    public float stepDistance = 0.5f;
    public float minStepInterval = 0.2f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    [Header("Ground Mask")]
    public static LayerMask GroundMask;

    private Rigidbody2D rb;
    private SurfaceSensor sensor;
    private float accumDistance;
    private Vector2 lastPos;
    private float lastStepTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sensor = GetComponentInChildren<SurfaceSensor>();
        GroundMask = LayerMask.GetMask("Ground");
        lastPos = rb.position;
    }

    private void Update()
    {
        float delta = Vector2.Distance(rb.position, lastPos);
        accumDistance += delta;
        lastPos = rb.position;

        if (accumDistance >= stepDistance
            && Time.time - lastStepTime >= minStepInterval
            && sensor != null
            && !string.IsNullOrEmpty(sensor.CurrentSurfaceTag))
        {
            MakeFootstep();
            lastStepTime = Time.time;
            accumDistance = 0f;
        }
    }

    void MakeFootstep()
    {
        AudioClip clip = null;
        switch (sensor.CurrentSurfaceTag)
        {
            case "Stone":
                if (stoneSteps.Length > 0)
                    clip = stoneSteps[Random.Range(0, stoneSteps.Length)];
                break;
            case "Grass":
                if (grassSteps.Length > 0)
                    clip = grassSteps[Random.Range(0, grassSteps.Length)];
                break;
            case "Wood":
                if (woodSteps.Length > 0)
                    clip = woodSteps[Random.Range(0, woodSteps.Length)];
                break;
                
        }

        if (clip != null)
        {
            float pitch = Random.Range(minPitch, maxPitch);
            AudioManager.Instance.PlayFootstep(clip, pitch);
        }
    }
}
