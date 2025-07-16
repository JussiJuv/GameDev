using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    [Tooltip("Name of the UI scene")]
    public string uiSceneName = "UI";

    private static bool _uiHasBeenLoaded = false;

    void Awake()
    {
        /*Debug.Log("[GameBootstrap]: Awake");

        if (_uiHasBeenLoaded)
        {
            SaveSystem.Load();

            SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);

            _uiHasBeenLoaded = true;
        }*/

        // Load the UI scene once
        if (!SceneManager.GetSceneByName(uiSceneName).isLoaded)
        {
            SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);
        }
    }
}
