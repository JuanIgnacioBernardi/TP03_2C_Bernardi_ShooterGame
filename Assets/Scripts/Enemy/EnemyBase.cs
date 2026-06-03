using UnityEngine;
using System.Collections;
[RequireComponent(typeof(HealthSystem))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] protected float detectionRange = 15f;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private float deathDelay = 3f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool facePlayer = true;

    protected Transform player;
    protected bool isDead;
    protected bool playerInRange;
    protected EnemyState _state = EnemyState.Idle;

    private HealthSystem _healthSystem;
    protected virtual void Start()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _healthSystem.onDie += HandleDeath;

        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
            player = playerGO.transform;
    }
    private void OnDestroy()
    {
        if (_healthSystem != null)
            _healthSystem.onDie -= HandleDeath;
    }
    protected virtual void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= detectionRange;

        if (playerInRange && !wasInRange)
            OnPlayerEnterRange();
        else if (!playerInRange && wasInRange)
            OnPlayerExitRange();

        if (playerInRange && facePlayer)
            FacePlayer();
    }
    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0f; 
        if (direction == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot,
                                               rotationSpeed * Time.deltaTime);
    }
    private void HandleDeath()
    {
        isDead = true;
        OnDeath();

        if (deathParticle != null)
        {
            deathParticle.transform.SetParent(null);
            deathParticle.Play();
        }

        StartCoroutine(DestroyAfterDeath());
    }
    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
    protected virtual void OnPlayerEnterRange() { }
    protected virtual void OnPlayerExitRange() { }
    protected virtual void OnDeath() { }
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}