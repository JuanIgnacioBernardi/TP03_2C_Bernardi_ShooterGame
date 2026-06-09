using UnityEngine;
[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Identity")]
    public EnemyAttackType attackType;

    [Header("Movement")]
    public float attackRange = 5f;
    public float moveSpeed = 3.5f;
    public float attackCooldown = 3f;

    [Header("Detection")]
    public float detectionRange = 15f;

    [Header("Combat")]
    public float shootingDamage = 10f;
    public float shootingSpeed = 2f;
    public float distanceToShoot = 15f;
    public float throwingDuration = 2f;
    public float shootingHeight = 3f;

    [Header("Health")]
    public float maxHealth = 100f;
}