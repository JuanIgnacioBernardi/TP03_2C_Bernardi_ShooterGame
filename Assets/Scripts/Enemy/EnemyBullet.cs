using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour, IPooleable
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float damage = 10f;

    private Rigidbody rb;
    private Coroutine returnCoroutine;
    private Coroutine arcCoroutine;
    public bool IsActive => gameObject.activeSelf;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
    public void Activate()
    {
        gameObject.SetActive(true);
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(ReturnAfterLifetime());
    }
    public void DeActivate()
    {
        if (returnCoroutine != null) { StopCoroutine(returnCoroutine); returnCoroutine = null; }
        if (arcCoroutine != null) { StopCoroutine(arcCoroutine); arcCoroutine = null; }
        rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }
    // Direct Shot
    public void Spawn(Vector3 position, Vector3 direction)
    {
        transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
        Activate();
        rb.linearVelocity = direction.normalized * speed;
    }
    // Parabolic arc for grenades
    public void Move(Vector3 startPos, Vector3 endPos, float duration, float height)
    {
        transform.position = startPos;
        Activate();
        if (arcCoroutine != null) StopCoroutine(arcCoroutine);
        arcCoroutine = StartCoroutine(ArcRoutine(startPos, endPos, duration, height));
    }
    private IEnumerator ArcRoutine(Vector3 start, Vector3 end, float duration, float height)
    {
        rb.isKinematic = true;
        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += height * 4f * t * (1f - t);
            transform.position = pos;
            time += Time.deltaTime;
            yield return null;
        }
        // Reached the destination & apply area damage as an explosion
        rb.isKinematic = false;
        arcCoroutine = null;

        // Area damage upon arrival
        Collider[] hits = Physics.OverlapSphere(transform.position, 3f);
        foreach (Collider hit in hits)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            target?.TakeDamage(damage);
        }
        DeActivate();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (arcCoroutine != null) return;
        IDamageable target = other.GetComponentInParent<IDamageable>();
        target?.TakeDamage(damage);
        DeActivate();
    }
    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(lifeTime);
        DeActivate();
    }
}