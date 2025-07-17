using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pausePanel, controlsPanel;

    public Button resumeBtn, loadGameBtn, controlsBtn, quitBtn;

    private GameObject _playerGO;
    private MonoBehaviour[] _toDisable;
    private Animator _playerAnim;

    public static PauseMenuController Instance { get; private set; }
    private bool isPaused = false;
    public bool IsPaused => isPaused;

    private void Awake()
    {
        Instance = this;

        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);

        resumeBtn.onClick.AddListener(Unpause);
        loadGameBtn.onClick.AddListener(OnLoadGame);
        controlsBtn.onClick.AddListener(() => controlsPanel.SetActive(true));

        var backBtn = controlsPanel.transform.Find("BackButton").GetComponent<Button>();
        backBtn.onClick.AddListener(() => controlsPanel.SetActive(false));
        quitBtn.onClick.AddListener(OnQuit);

        _playerGO = GameObject.FindWithTag("Player");
        if (_playerGO != null)
        {
            _toDisable = new MonoBehaviour[]
            {
                _playerGO.GetComponent<PlayerController>(),
                _playerGO.GetComponent<Weapon>(),
                _playerGO.GetComponent<ArrowRainAbility>(),
                _playerGO.GetComponent<AbilityManager>()
            };
            _playerAnim = _playerGO.GetComponent<Animator>();
        }

        Debug.Log("[PauseMenuController]: Pause menu created");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("[PauseMenuController]: Pause menu accessed");
            if (isPaused) Unpause();
            else Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // Disable scripts
        foreach (var mb in _toDisable)
            if (mb != null) mb.enabled = false;

        // Freeze animations
        if (_playerAnim != null)
            _playerAnim.speed = 0f;

        pausePanel.SetActive(true);
    }

    void Unpause()
    {
        foreach (var mb in _toDisable)
            if (mb != null) mb.enabled = true;

        if (_playerAnim != null)
            _playerAnim.speed = 1f;

        isPaused = false;
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void OnLoadGame()
    {
        Time.timeScale = 1f;
        SaveSystem.Load();
        var scene = SaveSystem.Data.lastScene;
        if (string.IsNullOrEmpty(scene))
            return;

        // Destroy all DDOL GameObjects
        foreach (var go in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (go.scene.buildIndex == -1 && go.GetComponent<PauseMenuController>() == null)
                Destroy(go);
        }

        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    private void OnQuit()
    {
        Time.timeScale = 1f;
        // Destroy any DDOL objects
        foreach (var go in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (go.scene.buildIndex == -1 && go.GetComponent<PauseMenuController>() == null)
                Destroy(go);
        }
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
