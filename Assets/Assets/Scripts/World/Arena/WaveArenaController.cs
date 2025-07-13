using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class WaveArenaController : MonoBehaviour
{
    [Header("Arena Detection")]
    [Tooltip("Trigger collider for arena interior")]
    public Collider2D arenaZone;
    private ContactFilter2D enemyFilter;
    private Collider2D[] overlapResults = new Collider2D[16];

    [Header("Wave Definitions")]
    [Tooltip("Configure each wave")]
    public WaveDefinition[] waves;

    [Header("Boss (last wave)")]
    [Tooltip("Spawn VFX for boss")]
    public GameObject spawnBoss;
    public GameObject bossPrefab;
    public float bossSpawnDelay = 1f;
    public float bossVFXPostDelay = 1f;

    [Header("Arena Flow Controller")]
    public EntranceWallController entranceWall;
    public StoneGateController gateController;
    public float waveTimeout = 10f;

    bool started = false;
    private WaveCounterUI waveCounterUI;

    private void Awake()
    {
        // Filter that only sees Enemy layer
        enemyFilter = new ContactFilter2D();
        enemyFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
        enemyFilter.useLayerMask = true;
    }

    private void Start()
    {
        waveCounterUI = FindFirstObjectByType<WaveCounterUI>();
        if (waveCounterUI == null)
        {
            Debug.LogError("[WaveArenaController] Could not find WaveCounterUI in any loaded scenes");
        }
        else
        {
            Debug.Log("!!!!!!!!!WaveCounterUI Found!!!!!!");
            waveCounterUI.Hide();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (started) return;
        if (!collision.CompareTag("Player")) return;

        started = true;
        entranceWall.SpawnWall();
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        int totalWaves = waves.Length;

        for (int i = 0; i < totalWaves; i++)
        {
            // TODO Update wave UI
            waveCounterUI.Show(i + 1, totalWaves);

            // Spawn normal enemies
            yield return SpawnWave(waves[i]);

            // spawn boss if final wave
            if (i == totalWaves - 1 && bossPrefab != null)
            {
                if (spawnBoss != null)
                    spawnBoss.SetActive(true);

                yield return new WaitForSeconds(waves[i].spawnDelay);

                if (spawnBoss != null)
                {
                    var anim = spawnBoss.GetComponent<Animator>();
                    if (anim != null)
                    {
                        anim.Play(0, 0, 0f);
                        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
                    }
                    //spawnBoss.SetActive(false);
                }
                
                yield return new WaitForSeconds(bossSpawnDelay);
                Instantiate(bossPrefab, spawnBoss.transform.position, Quaternion.identity);

                // keep VFX around for a bit longer
                yield return new WaitForSeconds(bossVFXPostDelay);

                if (spawnBoss != null)
                    spawnBoss.SetActive(false);

                // Final wave wait until there are no more enemies
                //yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
                yield return WaitForWaveClear();
                break;
            }
            else
            {
                // Not final wave: wait for clear
                yield return WaitForWaveClear();
            }
        }

        // Finished: Open the gate
        gateController.OpenGate();
        waveCounterUI?.Hide();
    }

    IEnumerator SpawnWave(WaveDefinition wave)
    {
        foreach (var entry in wave.spawns)
        {
            // Show spawnpoint VFX
            entry.spawnPoint.gameObject.SetActive(true);
            var anim = entry.spawnPoint.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play(0, 0, 0f);
                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            }
            entry.spawnPoint.gameObject.SetActive(false);

            // Spawn the enemy
            Instantiate(entry.prefab, entry.spawnPoint.position, Quaternion.identity);

            yield return new WaitForSeconds(wave.spawnDelay);
        }
    }

    IEnumerator WaitForWaveClear()
    {
        //float timer = 0f;
        while (true)
        {
            //int count = arenaZone.OverlapCollider(enemyFilter, overlapResults);
            int count = arenaZone.Overlap(enemyFilter, overlapResults);
            if (count == 0)
                break;

            yield return null;
        }
    }
}