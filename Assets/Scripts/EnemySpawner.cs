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
    // Find a valid position on the NavMesh near the given origin. If none found, returns the original position.
    private Vector3 GetNavMeshPosition(Vector3 origin, float searchRadius = 5f)
    {
        if (NavMesh.SamplePosition(origin, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
            return hit.position;

        Debug.LogWarning($"[EnemySpawner] No NavMesh encontrado cerca de {origin}. Usando posición original.");
        return origin;
    }
    public void SpawnEnemies(Transform player, CampController camp)
    {
        foreach (Transform sp in laserEnemySpawnPoints)
        {
            Vector3 pos = GetNavMeshPosition(sp.position);
            GameObject go = Instantiate(laserEnemyPrefab, pos, sp.rotation);
            go.transform.SetParent(transform);
            go.GetComponent<EnemyController>()?.Initialize(player);
            camp.RegisterEnemy(go.GetComponent<HealthSystem>());
        }
        foreach (Transform sp in grenadeEnemySpawnPoints)
        {
            Vector3 pos = GetNavMeshPosition(sp.position);
            GameObject go = Instantiate(grenadeEnemyPrefab, pos, sp.rotation);
            go.transform.SetParent(transform);
            go.GetComponent<EnemyController>()?.Initialize(player);
            camp.RegisterEnemy(go.GetComponent<HealthSystem>());
        }
        foreach (Transform sp in droneSpawnPoints)
        {
            if (droneEnemyPrefab == null) continue;
            GameObject go = Instantiate(droneEnemyPrefab, sp.position, sp.rotation);
            go.transform.SetParent(transform);
            go.GetComponent<DroneController>()?.Initialize(player);
            camp.RegisterEnemy(go.GetComponent<HealthSystem>());
        }
        foreach (Transform sp in turretSpawnPoints)
        {
            if (turretPrefab == null) continue;
            GameObject go = Instantiate(turretPrefab, sp.position, sp.rotation);
            go.transform.SetParent(transform);
            go.GetComponent<TurretController>()?.Initialize(player);
            camp.RegisterEnemy(go.GetComponent<HealthSystem>());
        }
    }
}