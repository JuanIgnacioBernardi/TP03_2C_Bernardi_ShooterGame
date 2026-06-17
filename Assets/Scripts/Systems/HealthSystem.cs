using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public event Action<float, float> onLifeChanged; // current health, max health
    public event Action onDie;
    public event Action OnDamaged;

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public bool isInvulnerable = false;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    private void Start()
    {
        onLifeChanged?.Invoke(currentHealth, maxHealth);
    }
    public void TakeDamage(float damage)
    {
        if (damage < 0 || isInvulnerable) return;
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            onDie?.Invoke();
        }
        else
        {
            OnDamaged?.Invoke();
            onLifeChanged?.Invoke(currentHealth, maxHealth);
        }
    }
    public void Heal(float amount)
    {
        if (amount <= 0 || currentHealth >= maxHealth) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onLifeChanged?.Invoke(currentHealth, maxHealth);
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        onLifeChanged?.Invoke(currentHealth, maxHealth);
    }
}