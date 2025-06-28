using UnityEngine;
using TMPro;
using System.Collections;

public class CoinPopup : MonoBehaviour
{
    public float duration = 2f;
    public Vector3 moveDistance = new Vector3(0, 30, 0);

    private TextMeshProUGUI tmp;
    private Color originalColor;

    void Awake()
    {
        // Try to get TMP on this object, or in its children
        tmp = GetComponent<TextMeshProUGUI>()
              ?? GetComponentInChildren<TextMeshProUGUI>();

        if (tmp == null)
        {
            Debug.LogError("CoinPopup: TextMeshProUGUI component not found on or under " + name);
            enabled = false;
            return;
        }

        originalColor = tmp.color;
    }

    public void Show(int amount)
    {
        tmp.text = amount > 0 ? "+" + amount : amount.ToString();
        StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.localPosition;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(startPos, startPos + moveDistance, t);
            tmp.color = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, 0), t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
