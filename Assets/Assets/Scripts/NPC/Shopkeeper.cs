/*using UnityEngine;
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

    *//*void Awake()
    {
        if (promptIcon == null) Debug.LogError("Shopkeeper: promptIcon not assigned.");

        // 1) Find the persistent UI Canvas
        var uiCanvas = GameObject.Find("UI Canvas");
        if (uiCanvas == null)
        {
            Debug.LogError("Shopkeeper: UI Canvas not found");
            return;
        }

        // 2) Find InventoryPanel ? ShopSection under it
        var panel = uiCanvas.transform.Find("InventoryPanel");
        if (panel == null)
        {
            Debug.LogError("Shopkeeper: InventoryPanel not found under UI Canvas");
            return;
        }
        var shopSection = panel.Find("ShopSection");
        if (shopSection == null)
        {
            Debug.LogError("Shopkeeper: ShopSection not found under InventoryPanel");
            return;
        }
        shopUI = shopSection.gameObject;

        // Ensure collider is a trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        promptOffset = new Vector3(0, promptYOffset, 0);

        // Hide prompt & shop UI initially
        if (promptIcon != null) promptIcon.gameObject.SetActive(false);
        if (shopUI != null) shopUI.SetActive(false);
    }*//*

    void Awake()
    {
        if (promptIcon == null)
            Debug.LogError("Shopkeeper: promptIcon not assigned.");

        // Leave shopUI null for now; we'll wire it in Start()
        GetComponent<Collider2D>().isTrigger = true;
        promptIcon.gameObject.SetActive(false);
    }

    void Start()
    {
        // Find the one persistent Canvas in memory
        var canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Shopkeeper: no Canvas found in any loaded scene!");
            return;
        }

        // Under that Canvas, find InventoryPanel -> ShopSection
        var panel = canvas.transform.Find("InventoryPanel");
        if (panel == null)
        {
            Debug.LogError("Shopkeeper: 'InventoryPanel' not found under Canvas");
            return;
        }

        var shopSection = panel.Find("ShopSection");
        if (shopSection == null)
        {
            Debug.LogError("Shopkeeper: 'ShopSection' not found under InventoryPanel");
            return;
        }

        shopUI = shopSection.gameObject;
        shopUI.SetActive(false);
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
            // Refresh inventory
            var invUI = FindFirstObjectByType<InventoryUI>();
            if (invUI != null) invUI.RefreshSlots();
        }

        Time.timeScale = nowOpen ? 0f : 1f;
    }
}
*/

// File: Assets/Assets/Scripts/NPC/Shopkeeper.cs

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

    private GameObject shopSection;   // child section under the panel
    private bool playerInRange = false;
    private Collider2D triggerCollider;
    private Vector3 promptOffset;

    void Awake()
    {
        if (promptIcon == null)
            Debug.LogError("Shopkeeper: promptIcon not assigned.");

        // Make this collider a trigger
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;

        // Hide the prompt immediately
        promptIcon.gameObject.SetActive(false);
    }

    void Start()
    {
        // 1) Find the persistent Canvas in any loaded scene
        var canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Shopkeeper: no Canvas found in any loaded scene!");
            return;
        }

        // 2) Under that Canvas, find the InventoryPanel
        var panelTf = canvas.transform.Find("InventoryPanel");
        if (panelTf == null)
        {
            Debug.LogError("Shopkeeper: 'InventoryPanel' not found under Canvas");
            return;
        }

        // 3) Assign the panel itself to shopUI
        shopUI = panelTf.gameObject;
        shopUI.SetActive(false);

        // 4) Find the ShopSection inside the panel
        var shopSectionTf = panelTf.Find("ShopSection");
        if (shopSectionTf == null)
        {
            Debug.LogError("Shopkeeper: 'ShopSection' not found under InventoryPanel");
            return;
        }

        shopSection = shopSectionTf.gameObject;
        shopSection.SetActive(false);
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
            promptIcon.transform.localPosition = Vector3.up * promptYOffset;
            promptIcon.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerInRange)
        {
            playerInRange = false;
            promptIcon.gameObject.SetActive(false);
            if (shopUI != null) shopUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void ToggleShop()
    {
        if (shopUI == null) return;

        // Toggle the entire InventoryPanel on/off
        bool nowOpen = !shopUI.activeSelf;
        shopUI.SetActive(nowOpen);

        // Ensure the ShopSection inside is visible
        if (shopSection != null)
            shopSection.SetActive(nowOpen);

        // Hide or show the prompt icon accordingly
        if (promptIcon != null)
            promptIcon.gameObject.SetActive(!nowOpen);

        // When opening, refresh inventory contents
        if (nowOpen)
        {
            var invUI = FindFirstObjectByType<InventoryUI>();
            if (invUI != null)
                invUI.RefreshSlots();
        }

        // Pause or unpause the game
        Time.timeScale = nowOpen ? 0f : 1f;
    }
}
