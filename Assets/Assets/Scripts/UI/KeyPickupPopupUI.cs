using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KeyPickupPopupUI : MonoBehaviour
{
    public static KeyPickupPopupUI Instance { get; private set; }

    [Header("Refs")]
    public CanvasGroup canvasGroup;
    public Image keyIcon;
    public TMP_Text pickupText;

    [Header("Timing")]
    public float displayTime = 2f;
    public float fadeTime = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Call to show the popup for this key.
    /// </summary>
    public void Show(KeyItemData keyData)
    {
        StopAllCoroutines();
        keyIcon.sprite = keyData.keyIcon;
        pickupText.text = $"You got a {keyData.keyName}";
        gameObject.SetActive(true);
        StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        // Fade in
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / fadeTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Wait
        yield return new WaitForSeconds(displayTime);

        // Fade out
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
