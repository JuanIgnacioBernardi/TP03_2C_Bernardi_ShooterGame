using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour, IPooleable
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage = 10f;

    private Rigidbody _rb;
    private Coroutine _returnCoroutine;
    public bool IsActive => gameObject.activeSelf;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
    public void Activate()
    {
        gameObject.SetActive(true);
        if (_returnCoroutine != null) StopCoroutine(_returnCoroutine);
        _returnCoroutine = StartCoroutine(ReturnAfterLifetime());
    }
    public void DeActivate()
    {
        if (_returnCoroutine != null)
        {
            StopCoroutine(_returnCoroutine);
            _returnCoroutine = null;
        }
        _rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
    public void Spawn(Vector3 position, Vector3 direction)
    {
        transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
        Activate();
        _rb.linearVelocity = direction.normalized * speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Damage player if touches them
        IDamageable target = other.GetComponentInParent<IDamageable>();
        target?.TakeDamage(damage);

        // Spawn impact if there is a bootstrapper
        if (GameBootstrapper.Instance != null)
        {
            ImpactParticle impact = GameBootstrapper.Instance.PoolManager.GetFromPool<ImpactParticle>();
            // Orient against the flight direction
            impact?.Spawn(transform.position, -transform.forward);
        }
        DeActivate();
    }
    public void Move(Vector3 startPos, Vector3 endPos, float duration, float height)
    {
        Activate();
        transform.position = startPos;
        if (_returnCoroutine != null) StopCoroutine(_returnCoroutine);
        _returnCoroutine = StartCoroutine(ArcRoutine(startPos, endPos, duration, height));
    }
    private IEnumerator ArcRoutine(Vector3 start, Vector3 end, float duration, float height)
    {
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += height * t * 4f * (1f - t); 
            transform.position = pos;
            time += Time.deltaTime;
            yield return null;
        }
        DeActivate();
    }
    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(lifeTime);
        DeActivate();
    }
}