using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    [Header("Starting scene")]
    public string sceneName = "demo";


    public Button newGameBtn, continueBtn, controlsBtn, quitBtn;
    public GameObject controlsPanel;
    [Tooltip("Back button from controlsPanel")]
    public Button backButton;

    private string savePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    private void Start()
    {
        newGameBtn.onClick.AddListener(OnNewGame);
        continueBtn.onClick.AddListener(OnContinue);
        controlsBtn.onClick.AddListener(() => controlsPanel.SetActive(true));
        quitBtn.onClick.AddListener(OnQuit);

        backButton.onClick.AddListener(() => controlsPanel.SetActive(false));

        // Disable Continue if no save file
        continueBtn.interactable = File.Exists(savePath);
    }

    void OnNewGame()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);
        // Force the SaveSystem to recreate defaults
        SaveSystem.Load();
        // Load first scene
        SceneManager.LoadScene(sceneName);
    }

    void OnContinue()
    {
        // Load lastScene from the existing save
        SaveSystem.Load();
        string scene = SaveSystem.Data.lastScene;
        if (!string.IsNullOrEmpty(scene))
            SceneManager.LoadScene(scene);
        else
            OnNewGame();
    }

    private void OnQuit()
    {
        Application.Quit();
    }
}
