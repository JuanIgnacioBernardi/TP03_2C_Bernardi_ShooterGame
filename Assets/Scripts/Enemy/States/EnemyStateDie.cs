using UnityEngine;
public class EnemyStateDie : EnemyStates
{
    public override void Initialize(Animator animator, EnemyController controller)
    {
        base.Initialize(animator, controller);
        state = StateTypeEnemy.Die;
    }

    public override void OnEnter()
    {
        _anim.SetInteger(HashState, (int)state);
    }
    public override void OnUpdate()
    {
        // Wait for the death animation to finish before deactivating the enemy
        AnimatorStateInfo info = _anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Death") && info.normalizedTime >= 1f)
            _controller.gameObject.SetActive(false);
    }
}