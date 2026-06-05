using UnityEngine;
public abstract class EnemyStates
{
    public StateTypeEnemy state;

    protected Animator _anim;
    protected EnemyController _controller;

    protected static readonly int HashState = Animator.StringToHash("State");
    public virtual void Initialize(Animator animator, EnemyController controller)
    {
        _anim = animator;
        _controller = controller;
    }
    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}