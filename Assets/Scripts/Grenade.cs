using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour, IPooleable
{
    [SerializeField] private float fuseTime = 3f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 60f;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private LayerMask damageMask;

    private Rigidbody _rb;
    private Coroutine _fuseCoroutine;
    public bool IsActive => gameObject.activeSelf;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
    public void Activate()
    {
        gameObject.SetActive(true);
        if (_fuseCoroutine != null) StopCoroutine(_fuseCoroutine);
        _fuseCoroutine = StartCoroutine(FuseRoutine());
    }
    public void DeActivate()
    {
        if (_fuseCoroutine != null)
        {
            StopCoroutine(_fuseCoroutine);
            _fuseCoroutine = null;
        }
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
    public void Spawn(Vector3 position, Vector3 velocity)
    {
        transform.position = position;
        Activate();
        _rb.linearVelocity = velocity;
    }
    private IEnumerator FuseRoutine()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }
    private void Explode()
    {
        if (explosionParticle != null)
        {
            explosionParticle.transform.SetParent(null);
            explosionParticle.Play();
        }

        // Radial damage
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
        foreach (Collider hit in hits)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            target?.TakeDamage(explosionDamage);
        }

        DeActivate();
    }
    // Visualize range of explotion
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}