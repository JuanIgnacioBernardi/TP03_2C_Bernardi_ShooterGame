using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyDataSO data;
    [SerializeField] private Transform shootPoint;

    [Header("Flight")]
    [SerializeField] private float flightHeight = 6f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float bobAmplitude = 0.3f;
    [SerializeField] private float bobFrequency = 1f;
    [SerializeField] private LayerMask groundMask;

    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 20f;
    [SerializeField] private float minPointDistance = 5f;

    [Header("Laser Feedback")]
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private float aimDuration = 1f;

    [Header("Death")]
    [SerializeField] private ParticleSystem explosionParticle;

    private float baseHeight;
    private float bobTimer;
    private Vector3 currentPatrolTarget;
    private DroneState state = DroneState.Patrol;
    private bool hasPatrolTarget;
    private bool isDead;
    private HealthSystem healthSystem;
    private Rigidbody rb;
    private Transform player;
    private Coroutine attackCoroutine;
    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // kinematic while alive, physics on death
        rb.useGravity = false;
    }
    private void OnEnable() => healthSystem.onDie += OnDie;
    private void OnDisable() => healthSystem.onDie -= OnDie;
    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;

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
        ApplyHoverBob();
        AdjustHeightToGround();
        switch (state)
        {
            case DroneState.Patrol: UpdatePatrol(); break;
            case DroneState.Follow: UpdateFollow(); break;
            case DroneState.Attack: UpdateAttack(); break;
        }
    }
    private void ApplyHoverBob()
    {
        bobTimer += Time.deltaTime * bobFrequency;
        float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
        Vector3 pos = transform.position;
        pos.y += bobOffset * Time.deltaTime;
        transform.position = pos;
    }
    // Raycasts down to detect ground and adjusts baseHeight
    private void AdjustHeightToGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            float targetY = hit.point.y + flightHeight;
            float newY = Mathf.MoveTowards(transform.position.y, targetY, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    private float GetHorizontalDistance()
    {
        if (player == null) return float.MaxValue;
        Vector3 flat = new Vector3(player.position.x, transform.position.y, player.position.z);
        return Vector3.Distance(transform.position, flat);
    }
    private void UpdatePatrol()
    {
        if (player != null && GetHorizontalDistance() < data.detectionRange)
        {
            state = DroneState.Follow;
            return;
        }
        if (!hasPatrolTarget || Vector3.Distance(transform.position, currentPatrolTarget) < 1.5f)
            GenerateNewPatrolPoint();
        MoveTowards(currentPatrolTarget);
    }
    private void GenerateNewPatrolPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 candidate = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        if (Vector3.Distance(transform.position, candidate) < minPointDistance)
        {
            Vector3 dir = (candidate - transform.position).normalized;
            candidate = transform.position + dir * minPointDistance;
        }
        currentPatrolTarget = candidate;
        hasPatrolTarget = true;
    }
    private void UpdateFollow()
    {
        if (player == null) return;
        float distance = GetHorizontalDistance();
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
        MoveTowards(player.position);
    }
    private void UpdateAttack()
    {
        if (player == null) return;
        if (GetHorizontalDistance() > data.attackRange)
        {
            state = DroneState.Follow;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
            if (laserRenderer != null) laserRenderer.enabled = false;
        }
        RotateTowards(player.position);
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
    // Shows laser for aimDuration seconds before shooting
    private IEnumerator AimRoutine()
    {
        if (laserRenderer != null) laserRenderer.enabled = true;
        float elapsed = 0f;
        while (elapsed < aimDuration)
        {
            elapsed += Time.deltaTime;
            if (player != null && laserRenderer != null)
            {
                laserRenderer.SetPosition(0, shootPoint.position);
                laserRenderer.SetPosition(1, player.position);
            }
            yield return null;
        }
        if (laserRenderer != null) laserRenderer.enabled = false;
    }
    private void ShootAtPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - shootPoint.position).normalized;

        if (Physics.Raycast(shootPoint.position, direction, out RaycastHit hit, data.distanceToShoot))
        {
            IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
            target?.TakeDamage(data.shootingDamage);
        }
    }
    private void MoveTowards(Vector3 target)
    {
        Vector3 flatTarget = new Vector3(target.x, transform.position.y, target.z);
        transform.position = Vector3.MoveTowards(transform.position, flatTarget, moveSpeed * Time.deltaTime);
        RotateTowards(flatTarget);
    }
    private void RotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction == Vector3.zero) return;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
    private void OnDie()
    {
        isDead = true;
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        if (laserRenderer != null) laserRenderer.enabled = false;

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.3f;
        rb.AddForce(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse);

        // Free rotation on death for a more dynamic look
        rb.constraints = RigidbodyConstraints.None;

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
        // Waits a moment before starting to fade out, allowing explosion to be visible
        yield return new WaitForSeconds(2f);

        // Fade out gradual — scale to zero in 1 second
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsed);
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