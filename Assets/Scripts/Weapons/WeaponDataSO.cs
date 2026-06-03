using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Identity")]
    public string weaponName;
    public Sprite hudIcon;

    [Header("Stats")]
    public float damage;
    public float range;
    public float fireRate;
    public float reloadTime;
    public int magazineSize;

    [Header("Muzzle")]
    public float muzzleLightDuration = 0.1f;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Header("Pickup")]
    public GameObject pickupPrefab; 
}