using UnityEngine;

public class PillarInteraction : MonoBehaviour
{
    [Tooltip("0 = first rune, 1 = second, etc.")]
    public int runeIndex;

    [Tooltip("Drag your Stone_Gate's PuzzleManager here")]
    public PuzzleManager manager;

    [Tooltip("Child GameObject with the ´Press E' icon")]
    public GameObject prompt;

    [Header("SFX")]
    public AudioClip clickSFX;

    private bool _playerInRange = false;

    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            AudioManager.Instance.PlaySFX(clickSFX);
            manager.TryActivate(runeIndex);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            prompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            prompt.SetActive(false);
        }
    }
}
