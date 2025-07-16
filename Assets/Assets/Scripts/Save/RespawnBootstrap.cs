using System.Collections;
using UnityEngine;

public class RespawnBootstrap : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;

        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO == null)
        {
            Debug.LogError("[RespawnBootstrap]: Player not found after scene load");
            Destroy(gameObject);
            yield break;
        }

        Transform playerTransform = playerGO.transform;
        Health playerHealth = playerGO.GetComponent<Health>();

        if (playerHealth == null)
        {
            Debug.LogError("[RespawnBootstrap]: Player has no health component after reloading");
            Destroy(gameObject);
            yield break;
        }

        if (!playerGO.activeSelf)
            playerGO.SetActive(true);

        playerHealth.Revive();

        var checkpointID = DeathUIController.RespawnData.PendingCheckpointID;
        var allCP = Object.FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        var target = System.Array.Find(allCP, cp => cp.checkpointID == checkpointID);

        if (target != null)
        {
            playerTransform.position = target.transform.position;
        }
        else
        {
            Debug.LogError($"[RespawnBootstrap]: Checkpoint '{checkpointID}' not found");
        }
        FindFirstObjectByType<SaveStateApplier>()?.ApplySavedState();

        var camFollow = FindFirstObjectByType<CameraFollow>();
        if (camFollow != null)
        {
            camFollow.target = playerTransform;
            camFollow.initialized = false;
            camFollow.ForceSnapToPlayer();
        }

        Destroy(gameObject);
    }
}
