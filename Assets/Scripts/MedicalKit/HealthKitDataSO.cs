using UnityEngine;
[CreateAssetMenu(fileName = "HealthKitData", menuName = "Player/HealthKitData")]
public class HealthKitDataSO : ScriptableObject
{
    [Header("Heal")]
    public float healPerSecond = 20f;
    public float healDuration = 3f;

    [Header("Pickup")]
    public float pickupRange = 10f;
    public Sprite hudIcon;
}