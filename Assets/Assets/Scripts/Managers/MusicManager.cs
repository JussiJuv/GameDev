using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Scene Music Clips")]
    public AudioClip demoMusic;
    public AudioClip hubMusic;
    public AudioClip area2Music;
    public AudioClip area2BossMusic;

    private AudioSource src;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            src = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
            UpdateMute();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        if (src == null)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            return;
        }

        AudioClip clip = null;
        switch (scene.name)
        {
            case "demo":
                clip = demoMusic;
                break;
            case "Hub":
                clip = hubMusic;
                break;
            case "Area2":
                clip = area2Music;
                break;
        }
        PlayClip(clip, true);
    }

    public void PlayBossMusic()
    {
        // Called from ActivateBoss()
        PlayClip(area2BossMusic, true);
    }

    private void PlayClip(AudioClip clip, bool loop)
    {
        // Already playing
        if (src.clip == clip)
            return;

        if (clip == null)
            return;

        src.loop = loop;
        src.clip = clip;
        src.time = 0f;
        src.Play();
    }

    public void UpdateMute()
    {
        if (src != null)
            src.mute = !AudioPreferences.MusicOn;
    }

}
