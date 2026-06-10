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

        // Stopping navmesh agent so it doesn't keep walking while attacking
        if (_controller.Agent != null)
        {
            _controller.Agent.ResetPath();
            _controller.Agent.isStopped = true;
        }
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
        if (_controller.Agent != null)
            _controller.Agent.isStopped = false;

        // Resets the Animator to the state that corresponds according to whether it can move or not
        _anim.SetInteger(HashState, _controller.CanMove ? (int)StateTypeEnemy.Roam : (int)StateTypeEnemy.Idle);
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