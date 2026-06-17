using System;
using UnityEngine;
public class CampController : MonoBehaviour
{
    public event Action<CampController> onCampCleared;

    private EnemySpawner spawner;
    private int totalEnemies;
    private int deadEnemies;
    private void Awake()
    {
        spawner = GetComponent<EnemySpawner>();
    }
    public void Initialize(Transform player)
    {
        spawner.SpawnEnemies(player, this);
    }
    public void RegisterEnemy(HealthSystem hs)
    {
        totalEnemies++;
        hs.onDie += OnEnemyDied;
    }
    private void OnEnemyDied()
    {
        deadEnemies++;
        Debug.Log($"[Camp] {gameObject.name} — {deadEnemies}/{totalEnemies} enemigos eliminados");

        if (deadEnemies >= totalEnemies)
        {
            Debug.Log($"[Camp] {gameObject.name} COMPLETADO");
            onCampCleared?.Invoke(this);
        }
    }
}