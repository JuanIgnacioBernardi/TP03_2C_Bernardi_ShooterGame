using System.Collections;
using UnityEngine;
[RequireComponent(typeof(EnemyController))]
public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private Transform _shootingPos;
    [SerializeField] private LineRenderer _laserRenderer;
    public Transform ShootingPos => _shootingPos;

    private EnemyController _controller;
    private Coroutine _coroutineAiming;
    private Coroutine _coroutineThrowing;
    private bool _isShooting;

    private static readonly int HashState = Animator.StringToHash("State");

    private void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }
    private void Start()
    {
        if (_laserRenderer != null)
        {
            _laserRenderer.startWidth = 0.03f;
            _laserRenderer.endWidth = 0.03f;
            _laserRenderer.positionCount = 2;
        }
    }
    private void OnDestroy() => StopAllCoroutines();

    // ── Laser ───────────────────
    public void AimAndShoot(bool active)
    {
        _isShooting = active;

        if (_coroutineAiming != null)
        {
            StopCoroutine(_coroutineAiming);
            _coroutineAiming = null;
        }
        if (_laserRenderer != null) _laserRenderer.enabled = false;

        if (active)
            _coroutineAiming = StartCoroutine(AimingRoutine());
    }
    private IEnumerator AimingRoutine()
    {
        Animator anim = _controller.GetAnim();
        while (_isShooting)
        {
            // Aim — Laser follow player
            if (_laserRenderer != null) _laserRenderer.enabled = true;
            anim.SetInteger(HashState, (int)StateTypeEnemy.Aim);

            float clock = 0f;
            float aimTime = _controller.Data.shootingSpeed / 2f;

            while (clock < aimTime)
            {
                clock += Time.deltaTime;

                if (_controller.Player != null)
                {
                    transform.LookAt(_controller.Player);
                    if (_laserRenderer != null)
                    {
                        _laserRenderer.SetPosition(0, _shootingPos.position);
                        _laserRenderer.SetPosition(1, _controller.Player.position + Vector3.up);
                    }
                }
                yield return null;
            }

            // Shoot — Turn off laser and raycast
            if (_laserRenderer != null) _laserRenderer.enabled = false;
            anim.SetInteger(HashState, (int)StateTypeEnemy.Attack);
            ShootRaycast();

            yield return new WaitForSeconds(_controller.Data.shootingSpeed);
        }
        if (_laserRenderer != null) _laserRenderer.enabled = false;
    }
    private void ShootRaycast()
    {
        if (_controller.Player == null) return;

        Vector3 targetPos = _controller.Player.position + Vector3.up * 1f; 
        Vector3 direction = (targetPos - _shootingPos.position).normalized;

        Debug.DrawRay(_shootingPos.position, direction * _controller.Data.distanceToShoot,
                      Color.red, 2f);

        if (Physics.Raycast(_shootingPos.position, direction,
            out RaycastHit hit, _controller.Data.distanceToShoot))
        {
            Debug.Log($"[EnemyShoot] Hit: {hit.collider.name}");
            IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
            target?.TakeDamage(_controller.Data.shootingDamage);
        }
        else
            Debug.Log("[EnemyShoot] No hit");
    }
    // ── Grenade ────────────
    public void ThrowObject(bool active, Transform startPos)
    {
        _isShooting = active;

        if (_coroutineThrowing != null)
        {
            StopCoroutine(_coroutineThrowing);
            _coroutineThrowing = null;
        }

        if (active)
            _coroutineThrowing = StartCoroutine(ThrowingRoutine(startPos));
    }
    private IEnumerator ThrowingRoutine(Transform startPos)
    {
        while (_isShooting)
        {
            yield return new WaitForSeconds(_controller.Data.shootingSpeed);

            if (GameBootstrapper.Instance == null) yield break;
            if (_controller.Player == null) yield break;

            EnemyBullet bullet = GameBootstrapper.Instance.PoolManager.GetFromPool<EnemyBullet>();
            if (bullet == null) { Debug.LogWarning("[EnemyShoot] Pool de EnemyBullet agotada."); yield break; }

            bullet.Activate();
            bullet.transform.position = startPos.position;

            Vector3 target = _controller.Player.position;
            Vector3 direction = (_controller.Player.position - startPos.position).normalized;
            bullet.Spawn(startPos.position, direction);
        }
    }
}