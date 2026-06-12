using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class TurretController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TurretDataSO data;

    [Header("References")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform muzzle;
    [SerializeField] private Animator headAnimator;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private LayerMask hitMask;

    private Transform player;
    private HealthSystem healthSystem;
    private bool isAiming;
    private bool isDead;

    private static readonly int HashShot = Animator.StringToHash("Shot");
    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        if (laserRenderer != null)
        {
            laserRenderer.startWidth = 0.03f;
            laserRenderer.endWidth = 0.03f;
            laserRenderer.positionCount = 2;
            laserRenderer.enabled = false;
        }
        if (muzzleFlash != null)
            muzzleFlash.Stop();
    }
    private void OnEnable() => healthSystem.onDie += OnDie;
    private void OnDisable() => healthSystem.onDie -= OnDie;
    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
    }
    private void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > data.detectionRange)
        {
            if (laserRenderer != null) laserRenderer.enabled = false;
            return;
        }
        RotateHeadToPlayer();

        // Aim laser while waiting to shoot
        if (laserRenderer != null)
        {
            laserRenderer.enabled = true;
            laserRenderer.SetPosition(0, muzzle.position);
            laserRenderer.SetPosition(1, player.position + Vector3.up);
        }
        if (!isAiming)
            StartCoroutine(ShootRoutine());
    }
    private void RotateHeadToPlayer()
    {
        Vector3 dir = (player.position - head.position);
        dir.y = 0f;
        Quaternion target = Quaternion.LookRotation(dir.normalized);
        head.rotation = Quaternion.Slerp(head.rotation, target, Time.deltaTime * data.rotationSpeed);
    }
    private IEnumerator ShootRoutine()
    {
        isAiming = true;
        yield return new WaitForSeconds(data.aimDuration);

        if (isDead || player == null) { isAiming = false; yield break; }

        SetLaser(false);
        PlayMuzzleEffects();
        ShootRaycast();

        yield return new WaitForSeconds(data.shootingCooldown);
        isAiming = false;
    }
    private void SetLaser(bool active)
    {
        if (laserRenderer != null) laserRenderer.enabled = active;
    }

    private void PlayMuzzleEffects()
    {
        headAnimator?.SetTrigger(HashShot);
        audioSource?.PlayOneShot(shootClip);
        if (muzzleFlash == null) return;
        muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        muzzleFlash.Play();
    }
    private void ShootRaycast()
    {
        Vector3 direction = (player.position + Vector3.up - muzzle.position).normalized;
        if (Physics.Raycast(muzzle.position, direction, out RaycastHit hit, data.distanceToShoot, hitMask))
            hit.collider.GetComponentInParent<IDamageable>()?.TakeDamage(data.shootingDamage);
    }
    private void OnDie()
    {
        isDead = true;
        StopAllCoroutines();

        if (laserRenderer != null) laserRenderer.enabled = false;

        StartCoroutine(DeathRoutine());
    }
    private IEnumerator DeathRoutine()
    {
        if (explosionParticle != null)
        {
            ParticleSystem explosion = Instantiate(explosionParticle,
                                                    transform.position,
                                                    Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration + 1f);
        }
        // Waits a bit before deactivating to let explosion play
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}