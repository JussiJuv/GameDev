using UnityEngine;
using System.Collections;

public class AltarController : MonoBehaviour
{
    [Header("Shard IDs")]
    public string shardA_ID = "shardA";
    public string shardB_ID = "shardB";

    [Header("Visuals")]
    public SpriteRenderer slotRenderer;
    public Sprite emptySlotSprite;
    public Sprite slotWithA;
    public Sprite slotWithBoth;
    public Animator activationAnimator;

    [Header("Prompt")]
    public SpriteRenderer promptIcon;
    public float promptYOfset = 1.5f;

    [Header("Fade")]
    public float fadeDuration = 1f;

    bool inRange, hasA, hasB, placedA, placedB;

    private void Start()
    {
        slotRenderer.sprite = emptySlotSprite;
        activationAnimator.gameObject.SetActive(false);

        if (promptIcon != null)
        {
            promptIcon.gameObject.SetActive(false);
            promptIcon.transform.localPosition = Vector3.up * promptYOfset;
        }
    }

    private void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            TryPlaceShard();
        }
    }

    private void TryPlaceShard()
    {
        var inv = PlayerInventory.Instance;
        // Place A first if the player has it and hasn't used it
        if (!placedA && inv.HasKey(shardA_ID))
        {
            inv.UseKey(shardA_ID);
            placedA = true;
            slotRenderer.sprite = slotWithA;

            // still in range and the player has shard B? keep prompt
            if (inRange && inv.HasKey(shardB_ID))
                promptIcon.gameObject.SetActive(true);
            else
                promptIcon.gameObject.SetActive(false);
            return;
        }

        // If A already placed, try B
        if (placedA && !placedB &&  inv.HasKey(shardB_ID))
        {
            inv.UseKey(shardB_ID);
            placedB = true;
            slotRenderer.sprite = slotWithBoth;
            promptIcon.gameObject.SetActive(false);

            // Both in place, activation anim
            StartCoroutine(ActivateAndCredits());
        }
    }

    IEnumerator ActivateAndCredits()
    {
        activationAnimator.gameObject.SetActive(true);
        activationAnimator.SetTrigger("Activate");
        yield return new WaitForSeconds(activationAnimator.GetCurrentAnimatorStateInfo(0).length);

        var fadeGo = GameObject.Find("UI Canvas/FadePanel");
        if (fadeGo == null)
            Debug.LogError("FadePanel not found under UI Canvas");
        else
        {
            var fadeGroup = fadeGo.GetComponent<CanvasGroup>();
            if (fadeGroup == null)
                Debug.LogError("FadePanel missing CanvasGroup");
            else
            {
                float t = 0f;
                fadeGroup.alpha = 0f;
                fadeGroup.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
            fadeGroup.alpha = 1f;
        }

        CreditsController.Instance.Show();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = true;
            if ((!placedA && PlayerInventory.Instance.HasKey(shardA_ID)) ||
                placedA && !placedB && PlayerInventory.Instance.HasKey(shardB_ID))
            {
                promptIcon?.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
            promptIcon?.gameObject.SetActive(false);
        }
    }
}
