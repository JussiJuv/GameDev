using UnityEngine;

public class Chest : MonoBehaviour
{
    [Tooltip("Unique ID for this chest")]
    public string chestID;

    [Header("Sprites & Prompt")]
    public GameObject closedSprite;     // child with closed chest
    public GameObject openSprite;       // child with open chest (initially inactive)
    public SpriteRenderer promptIcon;

    [Header("Chest Settings")]
    public int coinAmount = 10;
    public GameObject openVFX;
    public AudioClip openSFX;
    public float promptYOffset = 1f;

    [Header("Consumable Settings")]
    public ConsumableData[] consumableLoot;
    public int[] consumableCounts;

    private bool playerInRange = false;
    private bool isOpen = false;

    void Start()
    {
        if (!string.IsNullOrEmpty(chestID) && SaveSystem.Data.openedChests.Contains(chestID))
        {
            // Mark it opened from the start
            isOpen = true;
            closedSprite.SetActive(false);
            openSprite.SetActive(true);
            GetComponent<Collider2D>().enabled = false;
            promptIcon?.gameObject.SetActive(false);
            return;
        }

        // Otherwise normal init
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

        // Award consumables
        var inv = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        if (inv != null)
        {
            for (int i = 0; i < consumableLoot.Length; i++)
            {
                var data = consumableLoot[i];
                int count = consumableCounts[i];
                for (int c = 0; c < count; c++)
                {
                    inv.AddConsumable(data.type, 1);
                }
            }
        }
        else
        {
            Debug.LogWarning("[Chest]: PlayerInventory not found, no potions given");
        }

        // Save the state of the chest if its opened
        if (!string.IsNullOrEmpty(chestID))
        {
            SaveSystem.Data.openedChests.Add(chestID);
        }
        // Play VFX/SFX
        if (openVFX != null)
            Instantiate(openVFX, transform.position, Quaternion.identity);
        if (openSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(openSFX);

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
