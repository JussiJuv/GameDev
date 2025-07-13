using UnityEngine;
using System.Collections;
using TMPro;

public class WaveCounterUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TMP_Text counterText;
    [SerializeField] CanvasGroup canvasGroup;

    [Header("Fade Settings")]
    [SerializeField] float fadeDuration = 0.5f;

    private void Awake()
    {
        // Start hidden
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Show the counter and set text to "current/total"
    /// </summary>
    public void Show(int currentWave, int totalWaves)
    {
        counterText.text = $"{currentWave} / {totalWaves}";
        StopAllCoroutines();
        StartCoroutine(Fade(1f));
    }

    /// <summary>
    /// Update only the numbers
    /// </summary>
    public void UpdateCounter(int currentWave, int totalWaves)
    {
        counterText.text = $"{currentWave} /  {totalWaves}";
    }

    /// <summary>
    /// Fade out then hide
    /// </summary>
    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(0f));
    }

    IEnumerator Fade(float target)
    {
        float start = canvasGroup.alpha;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = target;
    }
}