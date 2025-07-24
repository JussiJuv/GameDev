using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AudioToggleButton : MonoBehaviour
{
    [Tooltip("Uncheck for SFX, check for Music")]
    public bool isMusicToggle = false;

    [Header("UI")]
    public Image iconImage;
    public Sprite onSprite;
    public Sprite offSprite;

    Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (iconImage == null)
            iconImage = GetComponent<Image>();
    }

    private void Start()
    {
        btn.onClick.AddListener(HandleClick);
        RefreshIcon();
    }

    void HandleClick()
    {
        if (isMusicToggle)
            AudioPreferences.MusicOn = !AudioPreferences.MusicOn;
        else
            AudioPreferences.SFXOn = !AudioPreferences.SFXOn;

        // If runtime managers are in scene, instantly mute/unmute them
        if (isMusicToggle && MusicManager.Instance != null)
            MusicManager.Instance.UpdateMute();
        if (!isMusicToggle && AudioManager.Instance != null)
            AudioManager.Instance.UpdateMute();

        RefreshIcon();
    }

    void RefreshIcon()
    {
        bool on = isMusicToggle
            ? AudioPreferences.MusicOn
            : AudioPreferences.SFXOn;
        iconImage.sprite = on ? onSprite : offSprite;
    }
}
