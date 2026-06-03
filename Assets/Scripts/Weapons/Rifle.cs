using UnityEngine;
using UnityEngine.InputSystem;

public class Rifle : WeaponBase
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
        damage = 25f;
        range = 100f;
        fireRate = 10f;
        reloadTime = 2.2f;
        magazineSize = 30;

        base.Awake();

    }
    private void Update()
    {
        if (Mouse.current.leftButton.isPressed)
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
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            Debug.Log($"Hit: {hit.collider.gameObject.name}");
            hit.collider.GetComponentInParent<IDamageable>()?.TakeDamage(damage);
        }
    }
    protected override void OnReloadStart()
    {
        if (audioSource != null && reloadSound != null) audioSource.PlayOneShot(reloadSound);
    }
}