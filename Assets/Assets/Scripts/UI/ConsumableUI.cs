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

    // Adapter for the ActiveConsumableChanged event
    private void OnActiveChanged(ConsumableType? newActive)
    {
        Refresh();
    }

    public void Refresh()
    {
        var inv = PlayerInventory.Instance;
        var active = inv.ActiveConsumable;
        Debug.Log($"[ConsumableUI]: Active = {active}");
        if (!active.HasValue)
        {
            slotUI.Clear();
            return;
        }
        var slot = inv.Consumables.First(s => s.type == active.Value);
        Debug.Log($"[ConsumableUI]: Slot count = {slot.count}");

        var data = inv.consumableDataList.FirstOrDefault(d => d.type == slot.type);
        if (data == null)
        {
            Debug.LogError($"[ConnsumableUI]: Missing ConsumableData for {slot.type}");
            slotUI.Clear();
            return;
        }
        Debug.Log($"[ConsumableUI]: Data.icon = {data.icon}");
        slotUI.SetItem(data.icon, slot.count);
    }
}
