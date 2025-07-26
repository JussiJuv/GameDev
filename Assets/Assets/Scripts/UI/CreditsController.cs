using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    public static CreditsController Instance { get; private set; }

    [Header("Refs")]
    public GameObject creditsPanel;
    public Transform contentContainer;
    public GameObject sectionPrefab;
    public Button closeButton;

    [Header("Auto Scroll")]
    public ScrollRect scrollRect;
    public float scrollDuration = 15f;
    public float startDelay = 2f;

    readonly string[] headings = { "Art", "SFX", "Music" };

    readonly string[][] entries = new[]
    {
        // Art
        new[]
        {
            "xXAshuraXx - Bow & Arrow",
            "Sangoro – Slime Enemy",
            "pimen – Dark Spell Effect",
            "Cainos – Pixel Art Top Down Basics",
            "karsiori – Pixel Art Key Pack Animated",
            "Szadi art. – Rogue Fantasy Catacombs",
            "egordorichev – Key Set",
            "La Red Games – Gems Coins",
            "Gif – Interior Pack",
            "Rian Nez – Potion in Glasses",
            "CreativeKind – Necromancer",
            "Foozle – Void Environment Pack"
        },
        // SFX
        new[]
        {
            "Pixabay",
            "ZapSplat",
            "nebula audio"
        },
        // Music
        new[]
        {
            "Katzen_Tupaz – Cloud of Sorrow",
            "Ebunny – Dragon Hunters Loop",
            "nickpanek620 – Heavy Trash Metal Instrumental",
            "Andorios – RPG Mistic Music Contrasting and Looped"
        }
    };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        creditsPanel.SetActive(false);
        closeButton.onClick.AddListener(Hide);
    }

    /// <summary>
    /// Call this to populate and show the credits.
    /// </summary>
    public void Show()
    {
        foreach (Transform ch in contentContainer)
            Destroy(ch.gameObject);

        for (int i = 0; i < headings.Length; i++)
        {
            var sec = Instantiate(sectionPrefab, contentContainer);
            var tmp = sec.GetComponent<TextMeshProUGUI>();
            tmp.text = $"<b>{headings[i]}</b>";
            tmp.fontSize = 24;

            foreach (var line in entries[i])
            {
                var ent = Instantiate(sectionPrefab, contentContainer);
                var t2 = ent.GetComponent<TextMeshProUGUI>();
                t2.text = line;
                t2.fontSize = 18;
            }
        }

        creditsPanel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AutoScroll());
    }

    private IEnumerator AutoScroll()
    {
        yield return new WaitForSeconds(startDelay);

        scrollRect.verticalNormalizedPosition = 1f;

        float elapsed = 0f;
        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(1f, 0f, elapsed / scrollDuration);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void Hide()
    {
        Time.timeScale = 1f;

        foreach (var go in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (go.scene.buildIndex == -1 && go != this.gameObject)
                Destroy(go);
        }

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
