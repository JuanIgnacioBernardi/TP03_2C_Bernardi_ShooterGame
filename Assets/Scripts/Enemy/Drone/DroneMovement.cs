using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    [SerializeField] private DroneDataSO data;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask obstacleMask;

    private float currentHeight;
    private float baseHeight;
    private float bobTimer;
    private Vector3 currentPatrolTarget;
    private bool hasPatrolTarget;
    private Rigidbody rb;
    private Vector3 spawnPoint;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        baseHeight = transform.position.y;
        currentHeight = baseHeight;
        spawnPoint = transform.position;
    }
    private void Start()
    {
        if (spawnPoint != Vector3.zero)
            SetNewPatrolTarget();
    }
    public void UpdateHeight()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, groundMask))
            currentHeight = Mathf.MoveTowards(currentHeight, hit.point.y + data.flightHeight, data.moveSpeed * Time.deltaTime);

        bobTimer += Time.deltaTime * data.bobFrequency;
        float bobOffset = Mathf.Sin(bobTimer) * data.bobAmplitude;

        Vector3 pos = transform.position;
        pos.y = currentHeight + bobOffset;
        transform.position = pos;
    }
    public void UpdateHeightCombat()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, groundMask))
            currentHeight = Mathf.MoveTowards(currentHeight, hit.point.y + data.combatFlightHeight, data.moveSpeed * Time.deltaTime);

        bobTimer += Time.deltaTime * data.bobFrequency;
        float bobOffset = Mathf.Sin(bobTimer) * data.bobAmplitude;

        Vector3 pos = transform.position;
        pos.y = currentHeight + bobOffset;
        transform.position = pos;
    }
    public void MoveTowards(Vector3 target)
    {
        Vector3 flatTarget = new Vector3(target.x, transform.position.y, target.z);
        Vector3 direction = (flatTarget - transform.position).normalized;
        float distance = data.moveSpeed * Time.deltaTime;

        // If there's an obstacle ahead, try to deflect around it
        if (Physics.SphereCast(transform.position, 0.8f, direction, out RaycastHit hit, 2f, obstacleMask))
        {
            Vector3 upDir = Vector3.up;
            transform.position += upDir * distance * 2f;
            return;
        }

        transform.position += direction * distance;
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
    public Vector3 GetNewPatrolPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * data.patrolRadius;
        Vector3 candidate = spawnPoint + new Vector3(randomCircle.x, 0f, randomCircle.y);

        if (Vector3.Distance(spawnPoint, candidate) < data.minPointDistance)
        {
            Vector3 dir = (candidate - spawnPoint).normalized;
            if (dir == Vector3.zero) dir = Vector3.forward;
            candidate = spawnPoint + dir * data.minPointDistance;
        }

        return candidate;
    }
    public bool HasReachedTarget(Vector3 target, float threshold = 1.5f)
    {
        Vector3 flatSelf = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 flatTarget = new Vector3(target.x, 0f, target.z);
        return Vector3.Distance(flatSelf, flatTarget) < threshold;
    }
    public float GetHorizontalDistance(Transform target)
    {
        if (target == null) return float.MaxValue;
        Vector3 flat = new Vector3(target.position.x, transform.position.y, target.position.z);
        return Vector3.Distance(transform.position, flat);
    }
    public Vector3 CurrentPatrolTarget => currentPatrolTarget;
    public bool HasPatrolTarget => hasPatrolTarget;
    public void InitializeSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
        SetNewPatrolTarget();
    }
    public void SetNewPatrolTarget()
    {
        currentPatrolTarget = GetNewPatrolPoint();
        currentPatrolTarget.y = spawnPoint.y;
        hasPatrolTarget = true;
    }
}