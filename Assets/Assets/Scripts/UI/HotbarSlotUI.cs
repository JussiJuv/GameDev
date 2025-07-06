using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlotUI : MonoBehaviour
{
    public Image iconImage;
    public Image cooldownOverlay;
    public TextMeshProUGUI keyLabel;

    private void Awake()
    {
        if (iconImage == null) iconImage = transform.Find("Icon").GetComponent<Image>();
        if (cooldownOverlay == null) cooldownOverlay = transform.Find("Icon/CooldownOverlay").GetComponent<Image>();
        if (keyLabel == null) keyLabel = transform.Find("KeyLabel").GetComponent<TextMeshProUGUI>();

        Debug.Log($"[HotbarSlotUI]: Awake on {name}, parent = {transform.parent?.name}");
    }

    public void Init(Sprite iconSprite, string key, Color disabledColor)
    {
        iconImage.sprite = iconSprite;
        iconImage.enabled = true;
        keyLabel.text = key;
        cooldownOverlay.fillAmount = 0f;
        iconImage.color = Color.white;
        keyLabel.color = Color.white;
    }

    public void SetCooldown(float normalized)
    {
        if (cooldownOverlay == null) 
        {
            Debug.LogError("[HotbarSlotUI]: cooldownOverlay is null");
            return;
        }
        cooldownOverlay.fillAmount = normalized;
        iconImage.color = Color.Lerp(Color.gray, Color.white, 1 - normalized);
    }

    public void Clear()
    {
        if (iconImage != null) iconImage.enabled = false;
        if (keyLabel != null) keyLabel.text = "";
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
    }
}
