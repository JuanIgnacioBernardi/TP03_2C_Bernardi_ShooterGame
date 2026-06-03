using System.Collections;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class LaserEnemy : EnemyBase
{
    [Header("Laser")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float aimDuration = 2f;
    [SerializeField] private float shootCooldown = 3f;

    [Header("Feedback")]
    [SerializeField] private Color aimColor = Color.red;
    [SerializeField] private Color readyColor = Color.yellow;

    private LineRenderer _laser;
    private Animator _animator;
    private Coroutine _attackCoroutine;

    private static readonly int HashPlayerInRange = Animator.StringToHash("playerInRange");
    private static readonly int HashShoot = Animator.StringToHash("shoot");
    private static readonly int HashDie = Animator.StringToHash("die");
    protected override void Start()
    {
        base.Start();
        _laser = GetComponent<LineRenderer>();
        _animator = GetComponentInChildren<Animator>();

        _laser.startWidth = 0.02f;
        _laser.endWidth = 0.02f;
        _laser.enabled = false;
    }
    protected override void OnPlayerEnterRange()
    {
        SetState(EnemyState.Alerted);
        _attackCoroutine = StartCoroutine(AttackRoutine());
    }
    protected override void OnPlayerExitRange()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
        _laser.enabled = false;
        SetState(EnemyState.Idle);
    }
    protected override void OnDeath()
    {
        _laser.enabled = false;
        if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
        SetState(EnemyState.Dead);
    }
    private void SetState(EnemyState newState)
    {
        _state = newState;

        switch (_state)
        {
            case EnemyState.Idle:
                _animator.SetBool(HashPlayerInRange, false);
                break;
            case EnemyState.Alerted:
            case EnemyState.Attacking:
                _animator.SetBool(HashPlayerInRange, true);
                break;
            case EnemyState.Dead:
                _animator.SetTrigger(HashDie);
                break;
        }
    }
    private IEnumerator AttackRoutine()
    {
        while (!isDead)
        {
            SetState(EnemyState.Alerted);
            _laser.enabled = true;
            _laser.startColor = aimColor;
            _laser.endColor = aimColor;

            float elapsed = 0f;
            while (elapsed < aimDuration)
            {
                if (player == null) yield break;

                _laser.SetPosition(0, firePoint.position);
                _laser.SetPosition(1, player.position);

                float t = elapsed / aimDuration;
                if (t > 0.6f)
                {
                    Color c = Color.Lerp(aimColor, readyColor, Mathf.PingPong(elapsed * 8f, 1f));
                    _laser.startColor = c;
                    _laser.endColor = c;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetState(EnemyState.Attacking);
            _animator.SetTrigger(HashShoot);
            _laser.enabled = false;
            Shoot();

            yield return new WaitForSeconds(shootCooldown);
        }
    }
    private void Shoot()
    {
        if (GameBootstrapper.Instance == null) return;

        EnemyBullet bullet = GameBootstrapper.Instance.PoolManager.GetFromPool<EnemyBullet>();
        if (bullet == null || player == null) return;

        Vector3 direction = (player.position - firePoint.position).normalized;
        bullet.Spawn(firePoint.position, direction);
    }
}