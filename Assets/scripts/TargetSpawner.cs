using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Option A — Pre-placed Targets")]
    public ShootingTarget[] targets;

    [Header("Option B — Prefab Spawning")]
    public GameObject targetPrefab;
    public Transform[] spawnPoints;

    private GameObject[] spawnedTargets;

    public void SpawnAll()
    {
        if (targets != null && targets.Length > 0)
        {
            foreach (ShootingTarget t in targets)
                if (t != null) t.gameObject.SetActive(true);
            return;
        }

        if (targetPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("TargetSpawner: No targets or prefab/spawnPoints assigned.");
            return;
        }

        spawnedTargets = new GameObject[spawnPoints.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnedTargets[i] = Instantiate(targetPrefab,
                                             spawnPoints[i].position,
                                             spawnPoints[i].rotation);
        }
    }

    public void DespawnAll()
    {
        if (targets != null && targets.Length > 0)
        {
            foreach (ShootingTarget t in targets)
                if (t != null) t.gameObject.SetActive(false);
        }

        if (spawnedTargets != null)
        {
            foreach (GameObject go in spawnedTargets)
                if (go != null) Destroy(go);
            spawnedTargets = null;
        }
    }
}
