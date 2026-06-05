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
        _anim.SetInteger(HashState, (int)state);

        switch (_controller.AttackType)
        {
            case EnemyAttackType.AimAndShoot:
                _controller.Shoot.AimAndShoot(true);
                break;
            case EnemyAttackType.ThrowObject:
                _controller.Shoot.ThrowObject(true, _controller.Shoot.ShootingPos);
                break;
            default:
                Debug.LogError("[EnemyStateAttack] AttackType no asignado.", _controller.gameObject);
                _controller.SwitchState(_controller.FindState(StateTypeEnemy.Idle));
                break;
        }
    }
    public override void OnUpdate()
    {
        // If player is not near, switch to idle state
        if (!_controller.CheckForNearPlayer())
            _controller.SwitchState(_controller.FindState(StateTypeEnemy.Idle));
    }
    public override void OnExit()
    {
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