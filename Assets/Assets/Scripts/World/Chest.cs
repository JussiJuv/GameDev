using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Sprites & Prompt")]
    public GameObject closedSprite;     // child with closed chest
    public GameObject openSprite;       // child with open chest (initially inactive)
    public SpriteRenderer promptIcon;   // child SpriteRenderer for the “Press E” icon

    [Header("Chest Settings")]
    public int coinAmount = 10;
    public GameObject openVFX;          // optional VFX prefab
    public AudioClip openSFX;           // optional sound clip
    public float promptYOffset = 1f;    // vertical offset for the prompt

    private bool playerInRange = false;
    private bool isOpen = false;

    void Start()
    {
        closedSprite.SetActive(true);
        openSprite.SetActive(false);
        promptIcon.gameObject.SetActive(false);

        // position the prompt just above chest
        if (promptIcon != null)
            promptIcon.transform.localPosition = Vector3.up * promptYOffset;
    }

    void Update()
    {
        if (playerInRange && !isOpen && Input.GetKeyDown(KeyCode.E))
            OpenChest();
    }

    private void OpenChest()
    {
        isOpen = true;

        // Swap sprites
        closedSprite.SetActive(false);
        openSprite.SetActive(true);

        // Hide the prompt
        if (promptIcon != null)
            promptIcon.gameObject.SetActive(false);

        // Award coins
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.AddCoins(coinAmount);

        // Play VFX/SFX
        if (openVFX != null)
            Instantiate(openVFX, transform.position, Quaternion.identity);
        if (openSFX != null)
            AudioSource.PlayClipAtPoint(openSFX, transform.position);

        // Disable further interaction
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            playerInRange = true;
            if (promptIcon != null) promptIcon.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            playerInRange = false;
            if (promptIcon != null) promptIcon.gameObject.SetActive(false);
        }
    }
}
