using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] laserEnemySpawnPoints;
    [SerializeField] private Transform[] grenadeEnemySpawnPoints;

    [Header("Prefabs")]
    [SerializeField] private GameObject laserEnemyPrefab;
    [SerializeField] private GameObject grenadeEnemyPrefab;
    private void Start()
    {
        SpawnLaserEnemies();
        SpawnGrenadeEnemies();
    }
    private void SpawnLaserEnemies()
    {
        foreach (Transform spawnPoint in laserEnemySpawnPoints)
        {
            GameObject go = Instantiate(laserEnemyPrefab,
                                        spawnPoint.position,
                                        spawnPoint.rotation);

            EnemyController controller = go.GetComponent<EnemyController>();
            controller.Initialize(playerTransform);
        }
    }
    private void SpawnGrenadeEnemies()
    {
        foreach (Transform spawnPoint in grenadeEnemySpawnPoints)
        {
            GameObject go = Instantiate(grenadeEnemyPrefab,
                                        spawnPoint.position,
                                        spawnPoint.rotation);

            EnemyController controller = go.GetComponent<EnemyController>();
            controller.Initialize(playerTransform);
        }
    }
}