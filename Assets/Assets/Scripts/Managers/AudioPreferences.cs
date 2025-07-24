using UnityEngine;

public class AudioPreferences : MonoBehaviour
{
    const string SfxKey = "SFXOn";
    const string MusicKey = "MusicOn";

    public static bool SFXOn
    {
        get => PlayerPrefs.GetInt(SfxKey, 1) == 1;
        set => PlayerPrefs.SetInt(SfxKey, value ? 1 : 0);
    }

    public static bool MusicOn
    {
        get => PlayerPrefs.GetInt(MusicKey, 1) == 1;
        set => PlayerPrefs.SetInt(MusicKey, value ? 1 : 0);
    }
}
