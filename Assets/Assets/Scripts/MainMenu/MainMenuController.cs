using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    [Header("Starting scene")]
    public string sceneName = "demo";

    public Button newGameBtn, continueBtn, controlsBtn, quitBtn, creditsBtn;
    public GameObject controlsPanel;
    [Tooltip("Back button from controlsPanel")]
    public Button backButton;

    [Header("Between-Level UI")]
    public StoryPanelController storyPanelController;

    private string savePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    private void Start()
    {
        newGameBtn.onClick.AddListener(OnNewGame);
        continueBtn.onClick.AddListener(OnContinue);
        controlsBtn.onClick.AddListener(() => controlsPanel.SetActive(true));
        quitBtn.onClick.AddListener(OnQuit);

        backButton.onClick.AddListener(() => controlsPanel.SetActive(false));

        creditsBtn.onClick.AddListener(OnCreditsPressed);

        // Disable Continue if no save file
        continueBtn.interactable = File.Exists(savePath);
    }

    void OnNewGame()
    {
        // Destroy old save
        if (File.Exists(savePath)) File.Delete(savePath);
        SaveSystem.Load();

        storyPanelController.Show(
            header: "Grasslands",
            body: "Your journey begins here. Defeat the Slime Giant to proceed.",
            onContinue: () =>
            {
                SceneManager.LoadScene(sceneName);
            });
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

    public void OnCreditsPressed()
    {
        string[] headings = { "Art", "SFX", "Music" };
        string[][] entries =
        {
            new[]
            {
                "Sangoro - Slime Enemy",
                "pimen - Dark Spell Effect",
                "Cainos - Pixel Art Top Down Basics",
                "karsiori - Pixel Art Key Pack Animated",
                "Szadi art. - Rogue Fantasy Catacombs",
                "egordorichev - Key Set",
                "La Red Games - Gems Coins",
                "Gif - Interior Pack",
                "Rian Nez - Potion in Glasses",
                "CreativeKind - Necromancer",
                "Foozle - Void Environment Pack"
            },
            new[]
            {
                "Pixabay",
                "ZapSplat",
                "nebula audio"
            },
            new[]
            {
                "Katzen_Tupaz - Cloud of Sorrow",
                "Ebunny - Dragon Hunters Loop",
                "nickpanek620 - Heavy Trash Metal Instrumental",
                "Andorios - RPG Mistic Music Contrasting and Looped"
            }
        };
        CreditsController.Instance.Show();
    }

    private void OnQuit()
    {
        Application.Quit();
    }
}
