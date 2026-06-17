using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
[RequireComponent(typeof(EnemyController))]
public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private Transform shootingPos;
    [SerializeField] private LineRenderer laserRenderer;
    public Transform ShootingPos => shootingPos;

    private EnemyController controller;
    private Coroutine coroutineAiming;
    private Coroutine coroutineThrowing;
    private bool isShooting;

    private static readonly int HashState = Animator.StringToHash("State");

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }
    private void Start()
    {
        if (laserRenderer != null)
        {
            laserRenderer.startWidth = 0.03f;
            laserRenderer.endWidth = 0.03f;
            laserRenderer.positionCount = 2;
        }
    }
    private void OnDestroy() => StopAllCoroutines();

    // ── Laser ───────────────────
    public void AimAndShoot(bool active)
    {
        isShooting = active;

        if (coroutineAiming != null)
        {
            StopCoroutine(coroutineAiming);
            coroutineAiming = null;
        }
        if (laserRenderer != null) laserRenderer.enabled = false;

        if (active)
            coroutineAiming = StartCoroutine(AimingRoutine());
    }
    private IEnumerator AimingRoutine()
    {
        Animator anim = controller.GetAnim();

        // Aim phase
        anim.SetInteger(HashState, (int)StateTypeEnemy.Aim);
        if (laserRenderer != null) laserRenderer.enabled = true;

        float clock = 0f;
        float aimTime = controller.Data.shootingSpeed / 2f;

        while (clock < aimTime)
        {
            clock += Time.deltaTime;
            if (controller.Player != null)
            {
                transform.LookAt(controller.Player);
                if (laserRenderer != null)
                {
                    laserRenderer.SetPosition(0, shootingPos.position);
                    laserRenderer.SetPosition(1, controller.Player.position + Vector3.up);
                }
            }
            yield return null;
        }

        // Shoot phase
        if (laserRenderer != null) laserRenderer.enabled = false;
        anim.SetInteger(HashState, (int)StateTypeEnemy.Attack);
        ShootRaycast();

        // Wait for attack animation
        yield return new WaitForSeconds(0.5f);

        // Back to idle — la FSM decide cuándo volver a atacar
        anim.SetInteger(HashState, (int)StateTypeEnemy.Idle);
        coroutineAiming = null;

        // Notifica al controller que terminó el ciclo
        controller.OnAttackCycleComplete();
    }
    private void ShootRaycast()
    {
        Debug.Log("[EnemyShoot] ShootRaycast called");
        if (controller.Player == null) return;

        Vector3 targetPos = controller.Player.position + Vector3.up * 1f; 
        Vector3 direction = (targetPos - shootingPos.position).normalized;

        Debug.DrawRay(shootingPos.position, direction * controller.Data.distanceToShoot,
                      Color.red, 2f);

        if (Physics.Raycast(shootingPos.position, direction,
            out RaycastHit hit, controller.Data.distanceToShoot))
        {
            Debug.Log($"[EnemyShoot] Hit: {hit.collider.name}");
            IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
            target?.TakeDamage(controller.Data.shootingDamage);
        }
        else
            Debug.Log("[EnemyShoot] No hit");
    }
    // ── Grenade ────────────
    public void ThrowObject(bool active, Transform startPos)
    {
        isShooting = active;

        if (coroutineThrowing != null)
        {
            StopCoroutine(coroutineThrowing);
            coroutineThrowing = null;
        }

        if (active)
            coroutineThrowing = StartCoroutine(ThrowingRoutine(startPos));
    }
    public void ThrowGrenade()
    {
        if (!isShooting || controller.Player == null) return;

        if (GameBootstrapper.Instance != null)
        {
            EnemyBullet bullet = GameBootstrapper.Instance.PoolManager.GetFromPool<EnemyBullet>();
            if (bullet != null)
                bullet.Move(shootingPos.position, controller.Player.position, controller.Data.throwingDuration, controller.Data.shootingHeight);
            else
                Debug.LogWarning("[EnemyShoot] Pool de EnemyBullet agotada.");
        }
    }
    private IEnumerator ThrowingRoutine(Transform startPos)
    {
        Animator anim = controller.GetAnim();

        while (isShooting)
        {
            if (controller.Player != null)
                transform.LookAt(controller.Player);

            anim.SetInteger(HashState, (int)StateTypeEnemy.Throw);

            // Event triggers ThrowGrenade()
            yield return new WaitForSeconds(controller.Data.shootingSpeed);

            anim.SetInteger(HashState, (int)StateTypeEnemy.Idle);
            yield return new WaitForSeconds(controller.Data.attackCooldown);
        }

        controller.GetAnim().SetInteger(HashState, (int)StateTypeEnemy.Idle);
        coroutineThrowing = null;
    }
}