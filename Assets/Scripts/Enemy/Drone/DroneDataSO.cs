using UnityEngine;
[CreateAssetMenu(fileName = "DroneData", menuName = "Enemy/DroneData")]
public class DroneDataSO : ScriptableObject
{
    [Header("Detection")]
    public float detectionRange = 20f;
    public float attackRange = 12f;

    [Header("Combat")]
    public float shootingDamage = 8f;
    public float shootingSpeed = 1.5f;
    public float distanceToShoot = 20f;
    public float aimDuration = 1f;

    [Header("Flight")]
    public float flightHeight = 6f;
    public float moveSpeed = 4f;
    public float rotationSpeed = 3f;
    public float bobAmplitude = 0.3f;
    public float bobFrequency = 1f;

    [Header("Patrol")]
    public float patrolRadius = 20f;
    public float minPointDistance = 5f;

    [Header("Health")]
    public float maxHealth = 60f;

    [Header("Death")]
    public float deathFallTime = 2f;
    public float fadeTime = 1f;
}