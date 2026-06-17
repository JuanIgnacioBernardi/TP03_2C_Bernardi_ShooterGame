using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HUDController : MonoBehaviour
{
    [Header("Damage Feedback")]
    [SerializeField] private DamageScreen damageScreen;

    [Header("Health Kit")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject kitIcon;
    [SerializeField] private TMP_Text kitCountText;

    [Header("Ammo")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text weaponNameText;

    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    [Header("References")]
    [SerializeField] private WeaponSwitcher weaponSwitcher;
    [SerializeField] private HealthSystem playerHealth;

    private WeaponBase _currentWeapon;
    private int _score;
    private void OnEnable()
    {
        playerInventory.onKitCountChanged += UpdateKitHUD;
        playerHealth.onLifeChanged += UpdateHealth;
        playerHealth.onDie += OnPlayerDie;
        playerHealth.OnDamaged += OnPlayerDamaged;
    }
    private void OnDisable()
    {
        playerInventory.onKitCountChanged -= UpdateKitHUD;
        playerHealth.onLifeChanged -= UpdateHealth;
        playerHealth.onDie -= OnPlayerDie;
        playerHealth.OnDamaged -= OnPlayerDamaged;
    }
    private void Start()
    {
        UpdateKitHUD(0);
        UpdateScore(0);
    }
    private void Update()
    {
        UpdateWeaponHUD();
    }
    private void OnPlayerDamaged()
    {
        damageScreen?.ShowDamage();
    }
    private void UpdateKitHUD(int count)
    {
        kitIcon.SetActive(count > 0);
        kitCountText.text = count > 0 ? $"x{count}" : "";
    }
    private void UpdateWeaponHUD()
    {
        WeaponBase weapon = weaponSwitcher.CurrentWeapon;
        if (weapon == null) return;
        if (weapon != _currentWeapon)
        {
            _currentWeapon = weapon;
            weaponNameText.text = weapon.Data != null ? weapon.Data.weaponName : weapon.name;
        }
        ammoText.text = $"{weapon.CurrentAmmo} / {weapon.MagazineSize}";
        ammoText.color = weapon.CurrentAmmo <= weapon.MagazineSize * 0.25f ? Color.red : Color.white;
    }
    private void UpdateHealth(float current, float max) 
    {
        damageScreen?.UpdateFromHealth(current, max);
    }
    private void OnPlayerDie()
    {
        ammoText.text = "";
        weaponNameText.text = "";
    }
    public void UpdateScore(int score)
    {
        _score = score;
        scoreText.text = $"SCORE: {_score}";
    }
    public void AddScore(int amount)
    {
        UpdateScore(_score + amount);
    }
}