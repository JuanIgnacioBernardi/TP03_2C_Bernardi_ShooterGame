using System.Collections;
using UnityEngine;
public class GrenadeEnemy : EnemyBase
{
    [Header("Granada")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwCooldown = 4f;
    [SerializeField] private float throwAngle = 45f;

    private Coroutine _throwCoroutine;
    protected override void OnPlayerEnterRange()
    {
        _throwCoroutine = StartCoroutine(ThrowRoutine());
    }
    protected override void OnPlayerExitRange()
    {
        if (_throwCoroutine != null)
        {
            StopCoroutine(_throwCoroutine);
            _throwCoroutine = null;
        }
    }
    protected override void OnDeath()
    {
        if (_throwCoroutine != null) StopCoroutine(_throwCoroutine);
    }
    private IEnumerator ThrowRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(throwCooldown);
            if (player != null)
                ThrowGrenade();
        }
    }
    private void ThrowGrenade()
    {
        if (GameBootstrapper.Instance == null) return;

        Grenade grenade = GameBootstrapper.Instance.PoolManager.GetFromPool<Grenade>();
        if (grenade == null || player == null) return;

        Vector3 velocity = CalculateThrowVelocity(throwPoint.position, player.position);
        grenade.Spawn(throwPoint.position, velocity);
    }
    private Vector3 CalculateThrowVelocity(Vector3 origin, Vector3 target)
    {
        Vector3 direction = target - origin;
        Vector3 dirXZ = new Vector3(direction.x, 0f, direction.z);
        float distance = dirXZ.magnitude;
        float angleRad = throwAngle * Mathf.Deg2Rad;

        float speed = Mathf.Sqrt(
            (distance * Mathf.Abs(Physics.gravity.y)) /
            Mathf.Sin(2f * angleRad)
        );

        Vector3 velocity = dirXZ.normalized * speed * Mathf.Cos(angleRad);
        velocity.y = speed * Mathf.Sin(angleRad);
        return velocity;
    }
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        if (throwPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(throwPoint.position, 0.2f);
        }
    }
}