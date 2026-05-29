using UnityEngine;

public class Enemy : MonoBehaviour
{
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.onDie += OnDie;
    }
    private void OnDestroy()
    {
        healthSystem.onDie -= OnDie;
    }
    private void OnDie()
    {
        Destroy(gameObject);
    }
}