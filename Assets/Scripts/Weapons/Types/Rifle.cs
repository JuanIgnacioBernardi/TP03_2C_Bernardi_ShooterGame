using UnityEngine;
using UnityEngine.InputSystem;

public class Rifle : WeaponBase
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private HitShake hitShake;

    [Header("Muzzle")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Light muzzleLight;

    private Coroutine muzzleLightCoroutine;
    protected override void Awake()
    {
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
        float lightDuration = data != null ? data.muzzleLightDuration : 0.1f;
        PlayMuzzleEffects(muzzleFlash, muzzleLight, lightDuration, ref muzzleLightCoroutine);

        if (audioSource != null && data != null && data.shootSound != null)
            audioSource.PlayOneShot(data.shootSound);

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
        {
            hitShake?.Shake();
            SpawnImpact(hit);
            hit.collider.GetComponentInParent<IDamageable>()?.TakeDamage(damage);
        }
    }
    protected override void OnReloadStart()
    {
        if (audioSource != null && data != null && data.reloadSound != null)
            audioSource.PlayOneShot(data.reloadSound);
    }
}