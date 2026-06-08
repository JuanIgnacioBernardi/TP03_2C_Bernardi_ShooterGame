using UnityEngine;
public class EnemyStateIdle : EnemyStates
{
    public override void Initialize(Animator animator, EnemyController controller)
    {
        base.Initialize(animator, controller);
        state = StateTypeEnemy.Idle;
    }
    public override void OnEnter()
    {
        _anim.SetInteger(HashState, (int)state);
    }
    public override void OnUpdate()
    {
        if (_controller.IsOnCooldown) return;
        if (_controller.CheckForNearPlayer())
            _controller.SwitchState(_controller.FindState(StateTypeEnemy.Attack));
    }
}