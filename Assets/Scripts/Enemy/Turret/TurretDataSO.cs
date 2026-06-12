using UnityEngine;
[CreateAssetMenu(fileName = "TurretData", menuName = "Data/TurretData")]
public class TurretDataSO : ScriptableObject
{
    [Header("Detection")]
    public float detectionRange = 20f;

    [Header("Shooting")]
    public float shootingDamage = 10f;
    public float shootingCooldown = 1.5f;
    public float distanceToShoot = 25f;
    public float aimDuration = 1f;

    [Header("Rotation")]
    public float rotationSpeed = 5f;
}