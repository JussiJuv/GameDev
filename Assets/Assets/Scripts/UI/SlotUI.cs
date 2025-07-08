using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public GameObject countTextObj;
    public TextMeshProUGUI countText;

    private bool isEmpty = true;

    public Button button => GetComponent<Button>();

    /// <summary>
    /// Set this slot to display the given item.
    /// </summary>
    public void SetItem(Sprite icon, int count = 1)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = true;
        }
        if (countTextObj != null)
        {
            //bool show = count > 1;
            bool show = true;
            countTextObj.SetActive(show);
            if (show && countText != null)
                countText.text = $"x{count}";
        }
        isEmpty = false;
    }

    /// <summary>
    /// Clear this slot to empty state.
    /// </summary>
    public void Clear()
    {
        if (iconImage != null)
            iconImage.enabled = false;
        if (countTextObj != null)
            countTextObj.SetActive(false);
        isEmpty = true;
    }

    /// <summary>
    /// Returns true if this slot has no item.
    /// </summary>
    public bool IsEmpty()
    {
        return isEmpty;
    }
}
