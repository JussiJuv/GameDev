using UnityEngine;
using System.Linq;

public class ConsumableUI : MonoBehaviour
{
    public SlotUI slotUI;

    private void Start()
    {
        var inv = PlayerInventory.Instance;
        if (inv == null)
        {
            Debug.LogError(("[ConsumableUI]: No PlayerInventory found in scene"));
            enabled = false;
            return;
        }

        inv.OnConsumablesChanged += Refresh;
        inv.OnActiveConsumableChanged += OnActiveChanged;
        Refresh();
    }

    void OnDestroy()
    {
        // Unsubscribe so we don't leak when this GameObject is destroyed
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnConsumablesChanged -= Refresh;
            PlayerInventory.Instance.OnActiveConsumableChanged -= OnActiveChanged;
        }
    }

    private void OnEnable()
    {
        var inv = PlayerInventory.Instance;
        inv.OnConsumablesChanged += Refresh;
        inv.OnActiveConsumableChanged += _ => Refresh();
        Refresh();
    }

    void OnDisable()
    {
        var inv = PlayerInventory.Instance;
        inv.OnConsumablesChanged -= Refresh;
        inv.OnActiveConsumableChanged -= _ => Refresh();
    }

    // Adapter for the ActiveConsumableChanged event
    private void OnActiveChanged(ConsumableType? newActive)
    {
        Refresh();
    }

    public void Refresh()
    {
        var inv = PlayerInventory.Instance;
        var active = inv.ActiveConsumable;
        if (!active.HasValue)
        {
            slotUI.Clear();
            return;
        }
        var slot = inv.Consumables.First(s => s.type == active.Value);

        var data = inv.consumableDataList.FirstOrDefault(d => d.type == slot.type);
        if (data == null)
        {
            Debug.LogError($"[ConnsumableUI]: Missing ConsumableData for {slot.type}");
            slotUI.Clear();
            return;
        }
        slotUI.SetItem(data.icon, slot.count);
    }
}
