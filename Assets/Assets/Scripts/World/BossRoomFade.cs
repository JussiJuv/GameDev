using UnityEngine;
using System.Collections;

public class BossRoomFade : MonoBehaviour
{
    [Tooltip("Material instance with our SpriteGrayscale shader")]
    public Material grayscaleMaterial;

    [Tooltip("How long to fade from gray?color")]
    public float fadeDuration = 1f;

    // Internal handle to the material property
    private static readonly int GrayProp = Shader.PropertyToID("_GrayAmount");

    public void Awake()
    {
        // Ensure we start fully gray
        if (grayscaleMaterial != null)
            grayscaleMaterial.SetFloat(GrayProp, 1f);
    }

    /// <summary>
    /// Kick off the grayscale?color fade.
    /// </summary>
    public void PlayFadeIn()
    {
        if (grayscaleMaterial != null)
            StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float amt = Mathf.Clamp01(1f - (t / fadeDuration));
            grayscaleMaterial.SetFloat(GrayProp, amt);
            yield return null;
        }
        // make sure its fully colored
        grayscaleMaterial.SetFloat(GrayProp, 0f);
    }
}
