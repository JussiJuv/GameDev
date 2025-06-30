using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    void Awake()
    {
        SaveSystem.Load();
    }
}
