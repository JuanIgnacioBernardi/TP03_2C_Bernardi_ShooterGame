using UnityEngine;
using UnityEngine.InputSystem;

public class Rifle : WeaponBase
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
        damage = 25f;
        range = 100f;
        fireRate = 10f;
        reloadTime = 2.2f;
        magazineSize = 30;

        base.Awake();

    }
    private void Start()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop();
        }
    }
    private void Update()
    {
        if (Mouse.current.leftButton.isPressed)
            TryShoot();
        else
            StopMuzzle();

        if (Keyboard.current.rKey.wasPressedThisFrame)
            TryReload();
    }
    protected override void Shoot()
    {
        if (muzzleFlash != null && !muzzleFlash.isPlaying) muzzleFlash.Play();
        if (audioSource != null && shootSound != null) audioSource.PlayOneShot(shootSound);

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            Debug.Log($"Hit: {hit.collider.gameObject.name} | Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
            target?.TakeDamage(damage);
        }
        else
        {
            Debug.Log("No hit");
        }
    }
    private void StopMuzzle()
    {
        if (muzzleFlash != null && muzzleFlash.isPlaying)
            muzzleFlash.Stop();
    }
    protected override void OnReloadStart()
    {
        if (audioSource != null && reloadSound != null) audioSource.PlayOneShot(reloadSound);
    }
}