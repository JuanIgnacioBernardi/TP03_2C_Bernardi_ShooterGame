using UnityEngine;
using UnityEngine.AI;
public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] laserEnemySpawnPoints;
    [SerializeField] private Transform[] grenadeEnemySpawnPoints;
    [SerializeField] private Transform[] droneSpawnPoints;
    [SerializeField] private Transform[] turretSpawnPoints;

    [Header("Prefabs")]
    [SerializeField] private GameObject laserEnemyPrefab;
    [SerializeField] private GameObject grenadeEnemyPrefab;
    [SerializeField] private GameObject droneEnemyPrefab;
    [SerializeField] private GameObject turretPrefab;
    private void Start()
    {
        SpawnLaserEnemies();
        SpawnGrenadeEnemies();
        SpawnDrones();
        SpawnTurrets();
    }
    // Find a valid position on the NavMesh near the given origin. If none found, returns the original position.
    private Vector3 GetNavMeshPosition(Vector3 origin, float searchRadius = 5f)
    {
        if (NavMesh.SamplePosition(origin, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
            return hit.position;

        Debug.LogWarning($"[EnemySpawner] No NavMesh encontrado cerca de {origin}. Usando posición original.");
        return origin;
    }
    private void SpawnLaserEnemies()
    {
        foreach (Transform spawnPoint in laserEnemySpawnPoints)
        {
            Vector3 pos = GetNavMeshPosition(spawnPoint.position);
            GameObject go = Instantiate(laserEnemyPrefab, pos, spawnPoint.rotation);
            go.GetComponent<EnemyController>()?.Initialize(playerTransform);
        }
    }
    private void SpawnGrenadeEnemies()
    {
        foreach (Transform spawnPoint in grenadeEnemySpawnPoints)
        {
            Vector3 pos = GetNavMeshPosition(spawnPoint.position);
            GameObject go = Instantiate(grenadeEnemyPrefab, pos, spawnPoint.rotation);
            go.GetComponent<EnemyController>()?.Initialize(playerTransform);
        }
    }
    private void SpawnDrones()
    {
        if (droneEnemyPrefab == null) return;
        foreach (Transform spawnPoint in droneSpawnPoints)
        {
            GameObject go = Instantiate(droneEnemyPrefab, spawnPoint.position, spawnPoint.rotation);
            go.GetComponent<DroneController>()?.Initialize(playerTransform);
        }
    }
    private void SpawnTurrets()
    {
        if (turretPrefab == null) return;
        foreach (Transform spawnPoint in turretSpawnPoints)
        {
            GameObject go = Instantiate(turretPrefab, spawnPoint.position, spawnPoint.rotation);
            go.GetComponent<TurretController>()?.Initialize(playerTransform);
        }
    }
}