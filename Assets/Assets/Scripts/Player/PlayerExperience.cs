using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    private void OnEnable()
    {
        if (XPManager.Instance != null)
        {
            XPManager.Instance.OnLevelUp += HandleLevelUp;
            XPManager.Instance.OnXPChanged += HandleXPChanged;
        }
    }

    private void HandleLevelUp(int newLevel)
    {
        // TODO: Award ability points, increase stats
        Debug.Log($"Player reached level {newLevel}!");
    }

    private void HandleXPChanged(int xp, int xpToNext)
    {
        // TODO: Update XP bar UI
        Debug.Log($"XP Updated: {xp} / {xpToNext}");
    }
}
