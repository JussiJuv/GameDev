using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [Header("Key & Door Setup")]
    public string doorID;       // Matches KeyItemData.doorID

    [Header("Sprites")]
    public GameObject closedSprite;
    public GameObject openSprite;

    [Header("Locked Message UI")]
    public Canvas messageCanvas;
    //public Text messageText;
    public TextMeshProUGUI messageText;
    public float messageDuration = 4f;

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
        }
        else
        {
            ShowMessage("The door is locked\nA Silver Key fits here");
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        closedSprite.SetActive(false);
        openSprite.SetActive(true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
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
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
