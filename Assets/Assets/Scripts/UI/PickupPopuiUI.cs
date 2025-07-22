using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PickupPopuiUI : MonoBehaviour
{
    public static PickupPopuiUI Instance { get; private set; }

    [Header("References")]
    public CanvasGroup canvasGroup;
    public Transform contentContainer;
    public GameObject entryPrefab;

    [Header("Timing")]
    public float displayTime = 3f;
    public float fadeTime = 0.5f;

    private readonly Queue<KeyItemData> _pending = new Queue<KeyItemData>();
    private bool _isShowing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void Show(IEnumerable<KeyItemData> items)
    {
        // make sure panel is active
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        StopAllCoroutines();

        foreach (var data in items)
            _pending.Enqueue(data);

        if (!_isShowing)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        _isShowing = true;
        while (_pending.Count > 0)
        {
            var data = _pending.Dequeue();

            foreach (Transform c in contentContainer)
                Destroy(c.gameObject);

            var go = Instantiate(entryPrefab, contentContainer);
            go.transform.Find("Icon").GetComponent<Image>().sprite = data.keyIcon;
            go.transform.Find("Label").GetComponent<TMP_Text>().text = data.keyName;

            // fade
            gameObject.SetActive(true);
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = t / fadeTime;
                yield return null;
            }
            canvasGroup.alpha = 1f;

            // Hold
            yield return new WaitForSeconds(displayTime);

            t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = 1f - (t / fadeTime);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }

        gameObject.SetActive(false);
        _isShowing = false;
    }

    /*public void Show(IEnumerable<KeyItemData> items)
    {
        StopAllCoroutines();
        foreach (Transform child in contentContainer)
            Destroy(child.gameObject);

        foreach (var data in items)
        {
            var go = Instantiate(entryPrefab, contentContainer);
            go.transform.Find("Icon").GetComponent<Image>().sprite = data.keyIcon;
            go.transform.Find("Label").GetComponent<TMP_Text>().text = $"{data.keyName}";
        }

        gameObject.SetActive(true);
        StartCoroutine(PopupRoutine());
    }*/

    private IEnumerator PopupRoutine()
    {
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / fadeTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

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
