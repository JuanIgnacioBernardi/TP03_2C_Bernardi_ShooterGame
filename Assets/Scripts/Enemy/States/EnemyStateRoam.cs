using UnityEngine;
using UnityEngine.AI;
public class EnemyStateRoam : EnemyStates
{
    private float roamRadius = 10f;
    private float waitTime = 2f;
    private float waitTimer;
    private bool isWaiting;
    public override void Initialize(Animator animator, EnemyController controller)
    {
        base.Initialize(animator, controller);
        state = StateTypeEnemy.Roam;
    }
    public override void OnEnter()
    {
        _anim.SetInteger(HashState, (int)StateTypeEnemy.Roam);
        isWaiting = false;
        SetNewDestination();
    }
    public override void OnUpdate()
    {
        if (_controller.CheckForNearPlayer())
        {
            _controller.SwitchState(_controller.FindState(StateTypeEnemy.Follow));
            return;
        }

        NavMeshAgent agent = _controller.Agent;
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                SetNewDestination();
            }
            return;
        }
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isWaiting = true;
            waitTimer = waitTime;
            _anim.SetInteger(HashState, (int)StateTypeEnemy.Idle);
        }
    }
    public override void OnExit()
    {
        _controller.Agent.ResetPath();
    }
    private void SetNewDestination()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere * roamRadius;
            randomDir += _controller.transform.position;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
            {
                _controller.Agent.SetDestination(hit.position);
                _anim.SetInteger(HashState, (int)StateTypeEnemy.Roam);
                return;
            }
        }
        // If no valid point found after 5 tries, just wait and try again later
        isWaiting = true;
        waitTimer = waitTime;
    }
}