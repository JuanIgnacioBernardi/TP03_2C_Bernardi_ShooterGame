using UnityEngine;
using UnityEngine.InputSystem;
public class Pistol : WeaponBase
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private LayerMask hitMask;

    [Header("Muzzle config")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Light muzzleLight;
    [SerializeField] private float muzzleLightDuration = 0.1f;

    private Coroutine muzzleLightCoroutine;

    protected override void Awake()
    {
        damage = 40f;
        range = 50f;
        fireRate = 3f;
        reloadTime = 1.5f;
        magazineSize = 12;

        base.Awake();
    }
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryShoot();

        if (Keyboard.current.rKey.wasPressedThisFrame)
            TryReload();
    }
    protected override void Shoot()
    {
        PlayMuzzleEffects(muzzleFlash, muzzleLight, muzzleLightDuration, ref muzzleLightCoroutine);

        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
            hit.collider.GetComponentInParent<IDamageable>()?.TakeDamage(damage);
    }
    protected override void OnReloadStart()
    {
        audioSource?.PlayOneShot(reloadSound);
    }
}