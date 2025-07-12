using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaveArenaController : MonoBehaviour
{
    [Header("Wave Definitions")]
    [Tooltip("Configure each wave")]
    public WaveDefinition[] waves;

    [Header("Boss (last wave)")]
    [Tooltip("Spawn VFX for boss")]
    public GameObject spawnBoss;
    [Tooltip("Boss enemy prefab to instantiate in last wave")]
    public GameObject bossPrefab;
    [Tooltip("Delay between boss VFX and boss spawn")]
    public float bossSpawnDelay = 1f;

    [Header("Arena Flow Controller")]
    public EntranceWallController entranceWall;
    public StoneGateController gateController;
    public float waveTimeout = 10f;

    bool started = false;

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
            // Second to last wave: show boss-VFX
            if (i == totalWaves - 2 && spawnBoss != null)
                spawnBoss.SetActive(true);

            // Normal enemy spawns for this wave
            yield return SpawnWave(waves[i]);

            // If this is the final wave, also spawn boss after the wave's monsters
            if (i == totalWaves - 1 && spawnBoss != null && bossPrefab != null)
            {
                // Wait so the last monster has time
                yield return new WaitForSeconds(waves[i].spawnDelay);

                // Play boss VFX
                var anim = spawnBoss.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.Play(0, 0, 0f);
                    yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
                }

                // Disable the VFX sprite and spawn the boss
                spawnBoss.SetActive(false);

                yield return new WaitForSeconds(bossSpawnDelay);
                Instantiate(bossPrefab, spawnBoss.transform.position, Quaternion.identity);
            }
            // Wait for all enemies to die
            yield return WaitForWaveClear(waveTimeout);
        }
        // Finished: Open the exit gate
        gateController.OpenGate();
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

    IEnumerator WaitForWaveClear(float timeout)
    {
        float timer = 0f;
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0 && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        // Proceed whether or not all enemies died, to avoid soft-lock
    }
}