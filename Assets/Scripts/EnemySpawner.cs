using UnityEngine;
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
    private void SpawnLaserEnemies()
    {
        foreach (Transform spawnPoint in laserEnemySpawnPoints)
        {
            GameObject go = Instantiate(laserEnemyPrefab, spawnPoint.position, spawnPoint.rotation);

            EnemyController controller = go.GetComponent<EnemyController>();
            controller.Initialize(playerTransform);
        }
    }
    private void SpawnGrenadeEnemies()
    {
        foreach (Transform spawnPoint in grenadeEnemySpawnPoints)
        {
            GameObject go = Instantiate(grenadeEnemyPrefab, spawnPoint.position, spawnPoint.rotation);

            EnemyController controller = go.GetComponent<EnemyController>();
            controller.Initialize(playerTransform);
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