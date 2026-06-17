using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DroneMovement))]
[RequireComponent(typeof(HealthSystem))]
public class DroneController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private DroneDataSO data;

    [Header("Sounds")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;

    [Header("References")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private LayerMask hitMask;

    private DroneState state = DroneState.Patrol;
    private DroneMovement movement;
    private HealthSystem healthSystem;
    private Rigidbody rb;
    private Transform player;
    private Coroutine attackCoroutine;
    private Vector3 pendingPosition;
    private bool hasPendingMove;
    private bool isDead;
    private void Awake()
    {
        movement = GetComponent<DroneMovement>();
        healthSystem = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    private void OnEnable() => healthSystem.onDie += OnDie;
    private void OnDisable() => healthSystem.onDie -= OnDie;
    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        movement.InitializeSpawnPoint(transform.position);

        if (laserRenderer != null)
        {
            laserRenderer.startWidth = 0.03f;
            laserRenderer.endWidth = 0.03f;
            laserRenderer.positionCount = 2;
            laserRenderer.enabled = false;
        }
    }
    private void Update()
    {
        if (isDead) return;

        switch (state)
        {
            case DroneState.Patrol:
                UpdatePatrol();
                movement.UpdateHeight();
                break;
            case DroneState.Follow:
                UpdateFollow();
                movement.UpdateHeightCombat();
                break;
            case DroneState.Attack:
                UpdateAttack();
                movement.UpdateHeightCombat();
                break;
        }
    }
    private void UpdatePatrol()
    {
        if (player != null && movement.GetHorizontalDistance(player) < data.detectionRange)
        {
            state = DroneState.Follow;
            return;
        }

        // Always patroll to the next target, if reached, get a new one
        if (!movement.HasPatrolTarget || movement.HasReachedTarget(movement.CurrentPatrolTarget))
            movement.SetNewPatrolTarget();

        movement.MoveTowards(movement.CurrentPatrolTarget);
    }
    private void UpdateFollow()
    {
        if (player == null) return;
        float distance = movement.GetHorizontalDistance(player);
        if (distance > data.detectionRange * 1.5f)
        {
            state = DroneState.Patrol;
            return;
        }
        if (distance < data.attackRange)
        {
            state = DroneState.Attack;
            if (attackCoroutine == null)
                attackCoroutine = StartCoroutine(AttackRoutine());
            return;
        }
        movement.MoveTowards(player.position);
    }
    private void UpdateAttack()
    {
        if (player == null) return;
        if (movement.GetHorizontalDistance(player) > data.attackRange)
        {
            state = DroneState.Follow;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
            if (laserRenderer != null) laserRenderer.enabled = false;
        }
        movement.RotateTowards(player.position);
    }
    private IEnumerator AttackRoutine()
    {
        while (state == DroneState.Attack && !isDead)
        {
            yield return StartCoroutine(AimRoutine());
            if (state != DroneState.Attack) break;
            ShootAtPlayer();
            yield return new WaitForSeconds(data.shootingSpeed);
        }
        if (laserRenderer != null) laserRenderer.enabled = false;
        attackCoroutine = null;
    }
    private IEnumerator AimRoutine()
    {
        if (laserRenderer != null) laserRenderer.enabled = true;
        float elapsed = 0f;

        while (elapsed < data.aimDuration)
        {
            elapsed += Time.deltaTime;

            if (player != null && laserRenderer != null)
            {
                Vector3 origin = shootPoint.position;
                Vector3 targetPos = player.position + Vector3.up * 1f;
                Vector3 direction = (targetPos - origin).normalized;

                // Laser cut off at obstacles
                if (Physics.Raycast(origin, direction, out RaycastHit hit, data.distanceToShoot))
                {
                    Debug.Log($"[Drone Laser] Hit: {hit.collider.name} | Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
                    laserRenderer.SetPosition(0, origin);
                    laserRenderer.SetPosition(1, hit.point);
                }
                else
                {
                    laserRenderer.SetPosition(0, origin);
                    laserRenderer.SetPosition(1, targetPos);
                }
            }
            yield return null;
        }

        if (laserRenderer != null) laserRenderer.enabled = false;
    }
    private void ShootAtPlayer()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + Vector3.up * 1f;
        Vector3 origin = shootPoint.position;
        Vector3 direction = (targetPos - origin).normalized;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, data.distanceToShoot, hitMask))
        {
            IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
            target?.TakeDamage(data.shootingDamage);
        }
    }
    private void OnDie()
    {
        AudioEvents.RaisePlaySFX(deathSound);
        isDead = true;
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        if (laserRenderer != null) laserRenderer.enabled = false;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.3f;
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse);

        StartCoroutine(DeathRoutine());
    }
    private IEnumerator DeathRoutine()
    {
        if (explosionParticle != null)
        {
            ParticleSystem explosion = Instantiate(explosionParticle, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration + 1f);
        }
        yield return new WaitForSeconds(data.deathFallTime);

        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;
        while (elapsed < data.fadeTime)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsed / data.fadeTime);
            yield return null;
        }

        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, data != null ? data.detectionRange : 10f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data != null ? data.attackRange : 5f);
    }
}