using UnityEditor.Rendering;
using UnityEngine;

public class FogLayerScroll : MonoBehaviour
{
    public Vector2 scrollSpeed = new Vector2(0.01f, 0.01f);
    private Vector2 offset;
    private Material mat;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        mat = sr.material;
        offset = mat.mainTextureOffset;
    }

    void Update()
    {
        offset += scrollSpeed * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }
}
