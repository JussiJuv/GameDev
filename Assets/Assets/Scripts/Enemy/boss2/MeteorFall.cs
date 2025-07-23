using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class MeteorFall : MonoBehaviour
{
    [Tooltip("How long the meteor takes to fall and the shadow to expand")]
    public float dropDuration = 1.0f;

    [Tooltip("Shadow prefab (with MeteorShadow.cs)")]
    public GameObject shadowPrefab;

    [Tooltip("Single?frame meteor sprite prefab")]
    public GameObject meteorSpritePrefab;

    [Tooltip("Impact animation prefab (with Animator & Collider)")]
    public GameObject impactPrefab;

    [Tooltip("Damage dealt if the player is within the impact collider")]
    public int damage = 2;

    [Header("SFX")]
    public AudioClip meteorHit;

    private Transform player;
    private Vector3 targetPos;

    /// <summary>
    /// Call this to start the meteor sequence.
    /// </summary>
    public void Init(Vector3 dropPosition, Transform playerTransform)
    {
        player = playerTransform;
        targetPos = dropPosition;

        // Spawn and configure the telegraphing shadow
        var shadow = Instantiate(shadowPrefab, targetPos, Quaternion.identity);
        shadow.GetComponent<MeteorShadow>().fallTime = dropDuration;

        // Spawn the meteor sprite high above the drop point
        Vector3 above = targetPos + Vector3.up * 10f;
        var spriteGO = Instantiate(meteorSpritePrefab, above, Quaternion.identity);

        StartCoroutine(FallSequence(spriteGO));
    }

    private IEnumerator FallSequence(GameObject spriteGO)
    {
        // the shadow and the meteor move over dropDuration
        Vector3 start = spriteGO.transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / dropDuration;
            // Move meteor down
            spriteGO.transform.position = Vector3.Lerp(start, targetPos, t);
            yield return null;
        }

        // On impact: remove the falling sprite
        Destroy(spriteGO);

        var impactGO = Instantiate(impactPrefab, targetPos, Quaternion.identity);

        // Enable its trigger so it can hurt the player
        var col = impactGO.GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // Attach logic to damage the player
        var impactScript = impactGO.AddComponent<MeteorImpact>();
        impactScript.damage = damage;

        var anim = impactGO.GetComponent<Animator>();
        if (anim != null)
        {
            float length = anim.GetCurrentAnimatorStateInfo(0).length;
            Destroy(impactGO, length + 0.1f);
        }
        else
        {
            Destroy(impactGO, 1.0f);
        }

        if (meteorHit != null)
            AudioManager.Instance.PlaySFX(meteorHit);

        Destroy(gameObject);
    }
}
