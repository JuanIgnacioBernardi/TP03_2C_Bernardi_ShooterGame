using UnityEngine;
public class EnemyStateFollow : EnemyStates
{
    public override void Initialize(Animator animator, EnemyController controller)
    {
        base.Initialize(animator, controller);
        state = StateTypeEnemy.Follow;
    }
    public override void OnEnter()
    {
        _anim.SetInteger(HashState, (int)StateTypeEnemy.Follow);
    }
    public override void OnUpdate()
    {
        if (_controller.Player == null) return;

        float distance = Vector3.Distance(_controller.transform.position, _controller.Player.position);

        // Far — back to roam
        if (distance > _controller.Data.detectionRange * 1.5f)
        {
            _controller.SwitchState(_controller.FindState(StateTypeEnemy.Roam));
            return;
        }
        // Atack if in range
        if (_controller.CheckForAttackRange())
        {
            _controller.SwitchState(_controller.FindState(StateTypeEnemy.Attack));
            return;
        }
        // Follow player
        _controller.Agent.SetDestination(_controller.Player.position);
    }
    public override void OnExit()
    {
        _controller.Agent.ResetPath();
    }
}