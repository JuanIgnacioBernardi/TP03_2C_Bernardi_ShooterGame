using UnityEngine;
public class EnemyStateAttack : EnemyStates
{
    public override void Initialize(Animator animator, EnemyController controller)
    {
        base.Initialize(animator, controller);
        state = StateTypeEnemy.Attack;
    }
    public override void OnEnter()
    {
        switch (_controller.AttackType)
        {
            case EnemyAttackType.AimAndShoot:
                _anim.SetInteger(HashState, (int)StateTypeEnemy.Aim);
                _controller.Shoot.AimAndShoot(true);
                break;
            case EnemyAttackType.ThrowObject:
                _anim.SetInteger(HashState, (int)StateTypeEnemy.Throw);
                _controller.Shoot.ThrowObject(true, _controller.Shoot.ShootingPos);
                break;
            default:
                Debug.LogError("[EnemyStateAttack] AttackType no asignado.");
                _controller.SwitchState(_controller.FindState(StateTypeEnemy.Idle));
                break;
        }

        if (_controller.Agent != null && _controller.Agent.enabled && _controller.Agent.isOnNavMesh)
        {
            _controller.Agent.ResetPath();
            _controller.Agent.isStopped = true;
        }
    }
    public override void OnUpdate()
    {
        if (!_controller.CheckForAttackRange())
        {
            _controller.OnAttackCycleComplete();

            if (_controller.CheckForNearPlayer())
                _controller.SwitchState(_controller.FindState(StateTypeEnemy.Follow));
            else
                _controller.SwitchState(_controller.FindState(
                    _controller.CanMove ? StateTypeEnemy.Roam : StateTypeEnemy.Idle));
        }
    }
    public override void OnExit()
    {
        if (_controller.Agent != null && _controller.Agent.enabled && _controller.Agent.isOnNavMesh)
        {
            _controller.Agent.isStopped = false;
            _controller.Agent.ResetPath();
        }
        _anim.SetInteger(HashState, _controller.CanMove ?
            (int)StateTypeEnemy.Follow : (int)StateTypeEnemy.Idle);

        switch (_controller.AttackType)
        {
            case EnemyAttackType.AimAndShoot:
                _controller.Shoot.AimAndShoot(false);
                break;
            case EnemyAttackType.ThrowObject:
                _controller.Shoot.ThrowObject(false, _controller.Shoot.ShootingPos);
                break;
        }
    }
}