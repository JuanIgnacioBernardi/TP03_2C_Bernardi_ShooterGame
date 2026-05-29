using UnityEngine;
using UnityEngine.InputSystem;
public class Pistol : WeaponBase
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private LayerMask hitMask;

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
        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && shootSound != null) audioSource.PlayOneShot(shootSound);

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
            target?.TakeDamage(damage);
        }
    }
    protected override void OnReloadStart()
    {
        audioSource?.PlayOneShot(reloadSound);
    }
}