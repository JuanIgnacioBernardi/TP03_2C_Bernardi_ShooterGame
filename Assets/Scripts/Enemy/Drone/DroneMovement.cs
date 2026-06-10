using UnityEngine;
public class DroneMovement : MonoBehaviour
{
    [SerializeField] private DroneDataSO data;
    [SerializeField] private LayerMask groundMask;

    private float bobTimer;
    private Vector3 currentPatrolTarget;
    private bool hasPatrolTarget;
    public void MoveTowards(Vector3 target)
    {
        Vector3 flatTarget = new Vector3(target.x, transform.position.y, target.z);
        transform.position = Vector3.MoveTowards(transform.position, flatTarget, data.moveSpeed * Time.deltaTime);
        RotateTowards(flatTarget);
    }
    public void RotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction == Vector3.zero) return;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, data.rotationSpeed * Time.deltaTime);
    }
    public void ApplyHoverBob()
    {
        bobTimer += Time.deltaTime * data.bobFrequency;
        float bobOffset = Mathf.Sin(bobTimer) * data.bobAmplitude;
        Vector3 pos = transform.position;
        pos.y += bobOffset * Time.deltaTime;
        transform.position = pos;
    }
    public void AdjustHeightToGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, groundMask))
        {
            float targetY = hit.point.y + data.flightHeight;
            float newY = Mathf.MoveTowards(transform.position.y, targetY, data.moveSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    public Vector3 GetNewPatrolPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * data.patrolRadius;
        Vector3 candidate = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        if (Vector3.Distance(transform.position, candidate) < data.minPointDistance)
        {
            Vector3 dir = (candidate - transform.position).normalized;
            candidate = transform.position + dir * data.minPointDistance;
        }
        return candidate;
    }
    public bool HasReachedTarget(Vector3 target, float threshold = 1.5f)
    {
        return Vector3.Distance(transform.position, target) < threshold;
    }
    public float GetHorizontalDistance(Transform target)
    {
        if (target == null) return float.MaxValue;
        Vector3 flat = new Vector3(target.position.x, transform.position.y, target.position.z);
        return Vector3.Distance(transform.position, flat);
    }
    public Vector3 CurrentPatrolTarget => currentPatrolTarget;
    public bool HasPatrolTarget => hasPatrolTarget;
    public void SetNewPatrolTarget()
    {
        currentPatrolTarget = GetNewPatrolPoint();
        hasPatrolTarget = true;
    }
}