using UnityEngine;
using UnityEngine.Events;
using System.Collections;

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

    private GameObject _playerGO;
    private MonoBehaviour[] _toDisableShop;
    private Animator _playerAnim;

    void Awake()
    {
        if (promptIcon == null)
            Debug.LogError("Shopkeeper: promptIcon not assigned.");

        // Make this collider a trigger
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;

        // Hide the prompt immediately
        promptIcon.gameObject.SetActive(false);

        _playerGO = GameObject.FindWithTag("Player");
        if (_playerGO != null)
        {
            _toDisableShop = new MonoBehaviour[]
            {
                _playerGO.GetComponent<PlayerController>(),
                _playerGO.GetComponent<Weapon>(),
                _playerGO.GetComponent<ArrowRainAbility>(),
                _playerGO.GetComponent<AbilityManager>()
            };
            _playerAnim = _playerGO.GetComponent<Animator>();
        }
    }

    void Start()
    {
        StartCoroutine(WaitForUICanvas());
    }

    void Update()
    {
        if (playerInRange
            && Input.GetKeyDown(KeyCode.E)
            && !(PauseMenuController.Instance?.IsPaused ?? false)
            && !(InventoryUI.Instance?.IsOpen ?? false))
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
            Time.timeScale = 0f;

            // disable player scripts
            if (_toDisableShop != null)
            {
                foreach (var mb in _toDisableShop)
                    if (mb != null)
                        mb.enabled = false;

                if (_playerAnim != null)
                    _playerAnim.speed = 0f;
            }

            var invUI = FindFirstObjectByType<InventoryUI>();
            if (invUI != null)
                invUI.RefreshSlots();
        }
        else
        {
            Time.timeScale = 1f;

            // enable player scripts
            if (_toDisableShop != null)
            {
                foreach (var mb in _toDisableShop)
                    if (mb != null)
                        mb.enabled = true;

                if (_playerAnim != null)
                    _playerAnim.speed = 1f;
            }
        }

        // Pause or unpause the game
        //Time.timeScale = nowOpen ? 0f : 1f;
    }

    IEnumerator WaitForUICanvas()
    {
        // Wait up to 1 second (arbitrary upper bound for safety)
        float timeout = 1f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            var canvas = GameObject.Find("UI Canvas");
            if (canvas != null)
            {
                var panelTf = canvas.transform.Find("InventoryPanel");
                if (panelTf != null)
                {
                    shopUI = panelTf.gameObject;
                    var shopSectionTf = panelTf.Find("ShopSection");
                    if (shopSectionTf != null)
                    {
                        shopSection = shopSectionTf.gameObject;
                        shopUI.SetActive(false);
                        shopSection.SetActive(false);
                        yield break; // success
                    }
                }
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        /*_playerGO = GameObject.FindWithTag("Player");
        if (_playerGO != null)
        {
            _toDisableShop = new MonoBehaviour[]
            {
                _playerGO.GetComponent<PlayerController>(),
                _playerGO.GetComponent<Weapon>(),
                _playerGO.GetComponent<ArrowRainAbility>(),
                _playerGO.GetComponent<AbilityManager>()
            };
            _playerAnim = _playerGO.GetComponent<Animator>();
        }*/

        Debug.LogError("[Shopkeeper] Failed to find UI Canvas and ShopSection within timeout.");
    }
}
