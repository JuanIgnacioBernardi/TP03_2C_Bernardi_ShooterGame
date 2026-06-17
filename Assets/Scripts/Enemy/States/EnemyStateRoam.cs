using UnityEngine;
using UnityEngine.AI;
public class EnemyStateRoam : EnemyStates
{
    private float waitTime = 2f;
    private float waitTimer;
    private bool isWaiting;

    // We save the enemy's spawn point so we don't stray too far.
    private Vector3 spawnPoint;
    private bool spawnCaptured;
    public override void Initialize(Animator animator, EnemyController controller)
    {
        base.Initialize(animator, controller);
        state = StateTypeEnemy.Roam;
    }
    public override void OnEnter()
    {
        _anim.SetInteger(HashState, (int)StateTypeEnemy.Roam);

        if (!spawnCaptured)
        {
            spawnPoint = _controller.transform.position;
            spawnCaptured = true;
        }

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
        float roamRadius = _controller.Data.patrolRadius;
        float[] radii = { roamRadius * 0.5f, roamRadius, roamRadius * 1.5f };

        foreach (float radius in radii)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector3 randomDir = Random.insideUnitSphere * radius;
                randomDir.y = 0f;
                randomDir += spawnPoint;

                if (!NavMesh.SamplePosition(randomDir, out NavMeshHit hit, radius, NavMesh.AllAreas))
                    continue;

                // Check that the path is complete, not partial
                NavMeshPath path = new NavMeshPath();
                _controller.Agent.CalculatePath(hit.position, path);
                if (path.status != NavMeshPathStatus.PathComplete)
                    continue;

                // Check that it is not too close to an obstacle
                if (NavMesh.FindClosestEdge(hit.position, out NavMeshHit edgeHit, NavMesh.AllAreas))
                {
                    if (edgeHit.distance < 1f) // very close to edge/wall
                        continue;
                }

                _controller.Agent.SetDestination(hit.position);
                _anim.SetInteger(HashState, (int)StateTypeEnemy.Roam);
                return;
            }
        }
        // Fallback: back to spawn point if no valid random point found
        if (NavMesh.SamplePosition(spawnPoint, out NavMeshHit spawnHit, 5f, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            _controller.Agent.CalculatePath(spawnHit.position, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                _controller.Agent.SetDestination(spawnHit.position);
                _anim.SetInteger(HashState, (int)StateTypeEnemy.Roam);
                return;
            }
        }
        isWaiting = true;
        waitTimer = waitTime;
    }
}