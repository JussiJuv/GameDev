using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    [Tooltip("Name of the UI scene")]
    public string uiSceneName = "UI";

    void Awake()
    {
        SaveSystem.Load();

        // Load the UI scene once
        if (!SceneManager.GetSceneByName(uiSceneName).isLoaded)
        {
            SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);
        }
    }
}
