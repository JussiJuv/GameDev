using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using TMPro;

public class StoryPanelController : MonoBehaviour
{
    public static StoryPanelController Instance { get; private set; }

    [Header("UI")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI headerText, bodyText;
    public Button continueButton;

    [Header("Timing")]
    public float fadeDuration = 1f;

    Action _onContinue;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Show the panel, load nextScene in background, then invoke onContinue when done.
    /// </summary>
    public void Show(string header, string body, string nextScene, Action onContinue = null)
    {
        gameObject.SetActive(true);
        headerText.text = header;
        bodyText.text = body;
        _onContinue = onContinue;

        // Start hidden
        canvasGroup.alpha = 0;
        Time.timeScale = 0f; // Pause gameplay

        // fade in
        StartCoroutine(FadeInAndLoad(nextScene));
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinuePressed);
        
    }

    IEnumerator FadeInAndLoad(string nextScene)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Begin async load
        var op = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        while (!op.isDone)
            yield return null;
    }

    void OnContinuePressed()
    {
        StartCoroutine(FadeOutAndContinue()); 
    }

    IEnumerator FadeOutAndContinue()
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.unscaledDeltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        _onContinue?.Invoke();
    }

}
