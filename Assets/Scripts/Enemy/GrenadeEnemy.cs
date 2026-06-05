using UnityEngine;

[RequireComponent(typeof(EnemyController), typeof(HealthSystem))]
public class GrenadeEnemy : MonoBehaviour, IPooleable
{
    public bool IsActive { get; private set; }
    public void Activate()
    {
        IsActive = true;
        gameObject.SetActive(true);
    }
    public void DeActivate()
    {
        IsActive = false;
        gameObject.SetActive(false);
    }
}