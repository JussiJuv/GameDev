using UnityEngine;

// When the player collides, it adds the key to the inventory and destroys itself.

[RequireComponent(typeof(Collider2D))]
public class KeyPickup : MonoBehaviour
{
    public KeyItemData keyData;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.AddKey(keyData);
                // TODO SFX
                Destroy(gameObject);
            }
        }
    }
}
