using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [Header("Key & Door Setup")]
    public string doorID;

    [Header("Sprites")]
    public GameObject closedSprite;
    public GameObject openSprite;
    public SpriteRenderer promptIcon;

    [Header("SFX")]
    public AudioClip doorSFX;


    [Header("Locked Message UI")]
    public Canvas messageCanvas;
    //public Text messageText;
    public TextMeshProUGUI messageText;
    public float messageDuration = 4f;
    [TextArea]
    public string lockedMessage = "Locked";

    private bool isOpen = false;
    private bool playerInRange = false;

    private void Start()
    {
        closedSprite.SetActive(true);
        openSprite.SetActive(false);
        messageCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        // When in range, listen for E to try opening
        if (playerInRange && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            TryOpen();
        }
    }

    private void TryOpen()
    {
        //PlayerInventory inv = FindObjectOfType<PlayerInventory>();
        PlayerInventory inv = FindFirstObjectByType<PlayerInventory>();
        if (inv != null && inv.HasKey(doorID))
        {
            inv.UseKey(doorID);
            OpenDoor();

            // Hide the promtp
            if (promptIcon != null)
            {
                promptIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            //ShowMessage("The door is locked\nA Silver Key fits here");
            ShowMessage(lockedMessage);
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        closedSprite.SetActive(false);
        openSprite.SetActive(true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        AudioManager.Instance.PlaySFX(doorSFX);
    }

    private void ShowMessage(string msg)
    {
        StopAllCoroutines();
        messageText.text = msg;
        messageCanvas.gameObject.SetActive(true);
        StartCoroutine(HideMessage());
    }

    private IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(messageDuration);
        messageCanvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpen)
        {
            playerInRange = true;
            if (promptIcon != null)
                promptIcon.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptIcon != null)
                promptIcon.gameObject.SetActive(false);
        }
    }
}
