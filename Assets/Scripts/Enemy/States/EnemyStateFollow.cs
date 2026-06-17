using UnityEngine;
public class EnemyStateFollow : EnemyStates
{
    private float enterTime;
    private StateTypeEnemy lastAnimState = StateTypeEnemy.Idle;

    public override void Initialize(Animator animator, EnemyController controller)
    {
        base.Initialize(animator, controller);
        state = StateTypeEnemy.Follow;
    }
    public override void OnEnter()
    {
        _anim.SetInteger(HashState, (int)StateTypeEnemy.Follow);
        lastAnimState = StateTypeEnemy.Follow;
        enterTime = UnityEngine.Time.time;
    }
    public override void OnUpdate()
    {
        if (_controller.Player == null) return;

        float distance = Vector3.Distance(_controller.transform.position, _controller.Player.position);

        if (distance > _controller.Data.detectionRange * 1.5f)
        {
            _controller.SwitchState(_controller.FindState(StateTypeEnemy.Roam));
            return;
        }
        if (_controller.CheckForAttackRange())
        {
            _controller.SwitchState(_controller.FindState(StateTypeEnemy.Attack));
            return;
        }

        _controller.Agent.SetDestination(_controller.Player.position);

        if (Time.time - enterTime > 0.1f)
        {
            float speed = _controller.Agent.velocity.magnitude;
            StateTypeEnemy targetAnim = speed > 0.1f ? StateTypeEnemy.Follow : StateTypeEnemy.Idle;

            if (targetAnim != lastAnimState)
            {
                lastAnimState = targetAnim;
                _anim.SetInteger(HashState, (int)targetAnim);
            }
        }
    }
    public override void OnExit()
    {
        _controller.Agent.ResetPath();
    }
}