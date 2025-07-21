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

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Show the panel, load nextScene in background, then invoke onContinue when done.
    /// </summary>
    public void Show(string header, string body, Action onContinue = null)
    {
        headerText.text = header;
        bodyText.text = body;
        _onContinue = onContinue;

        // Start hidden
        canvasGroup.alpha = 0;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f; // Pause gameplay

        // fade in
        //StartCoroutine(FadeInAndLoad(nextScene));
        StartCoroutine(FadeIn());
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(OnContinuePressed);
        
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    /*IEnumerator FadeInAndLoad(string nextScene)
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
    }*/

    void OnContinuePressed()
    {
        StartCoroutine(FadeOutAndContinue()); 
    }

    IEnumerator FadeOutAndContinue()
    {
        /*float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.unscaledDeltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        Time.timeScale = 1f;*/
        Time.timeScale = 1f;
        _onContinue?.Invoke();
        yield return null;
    }

}
