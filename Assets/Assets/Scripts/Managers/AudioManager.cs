using UnityEditor.Rendering;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource sfxSource;
    private AudioSource footStepSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            var sources = GetComponents<AudioSource>();
            if (sources.Length < 2)
                Debug.LogError("[AudioManager]: Need two AudioSources");

            sfxSource = sources[0];
            footStepSource = sources[1];
            /*//DontDestroyOnLoad(gameObject);
            if (sfxSource == null) 
                sfxSource = GetComponent<AudioSource>();

            if (sfxSource == null)
                Debug.LogError("[AudioManager]: No AudioSource assigned or found");*/

        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayFootstep(AudioClip clip, float pitch = 1f)
    {
        if (clip == null || footStepSource == null) return;

        footStepSource.pitch = pitch;
        footStepSource.PlayOneShot(clip);
        footStepSource.pitch = 1f;

        /*sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
        sfxSource.pitch = 1f;*/
    }
}
