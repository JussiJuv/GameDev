using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimator : MonoBehaviour
{
    [Tooltip("Frames of the animation, in order.")]
    public Sprite[] frames;
    [Tooltip("Frames per second")]
    public float fps = 10f;

    private Image img;
    private float timer;
    private int index;

    void Awake()
    {
        img = GetComponent<Image>();
        if (frames == null || frames.Length == 0)
            Debug.LogWarning("No frames assigned for UISpriteAnimator on " + name);
    }

    void Update()
    {
        if (frames == null || frames.Length == 0 || img == null) return;

        timer += Time.unscaledDeltaTime;
        float frameTime = 1f / fps;
        if (timer >= frameTime)
        {
            timer -= frameTime;
            index = (index + 1) % frames.Length;
            img.sprite = frames[index];
        }
    }
}
