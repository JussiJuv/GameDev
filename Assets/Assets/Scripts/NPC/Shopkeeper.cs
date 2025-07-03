using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Shopkeeper : MonoBehaviour
{
    [Header("Prompt Icon")]
    [Tooltip("SpriteRenderer for the “Press E” icon")]
    public SpriteRenderer promptIcon;
    [Tooltip("Vertical offset from shopkeeper's position")]
    public float promptYOffset = 1.5f;

    [Header("Shop UI")]
    [Tooltip("Canvas GameObject that contains the shop menu UI")]
    public GameObject shopUI;

    Collider2D triggerCollider;
    Vector3 promptOffset;
    private bool playerInRange = false;

    void Awake()
    {
        if (promptIcon == null) Debug.LogError("Shopkeeper: promptIcon not assigned.");
        if (shopUI == null) Debug.LogError("Shopkeeper: shopUI not assigned.");

        // Ensure collider is a trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        promptOffset = new Vector3(0, promptYOffset, 0);

        // Hide prompt & shop UI initially
        if (promptIcon != null) promptIcon.gameObject.SetActive(false);
        if (shopUI != null) shopUI.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerInRange)
        {
            playerInRange = true;
            if (promptIcon != null)
            {
                promptIcon.transform.localPosition = Vector3.up * promptYOffset;
                promptIcon.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerInRange)
        {
            playerInRange = false;
            if (promptIcon != null) promptIcon.gameObject.SetActive(false);
            if (shopUI != null) shopUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void ToggleShop()
    {
        if (shopUI == null) return;

        bool nowOpen = !shopUI.activeSelf;
        shopUI.SetActive(nowOpen);
        // hide prompt while shop is open
        if (promptIcon != null) promptIcon.gameObject.SetActive(!nowOpen);

        if (nowOpen && shopUI.transform.Find("ShopSection") is Transform ss)
        {
            ss.gameObject.SetActive(true);
        }

        Time.timeScale = nowOpen ? 0f : 1f;
    }
}
