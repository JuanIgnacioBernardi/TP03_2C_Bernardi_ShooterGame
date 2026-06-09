using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(EnemyShoot), typeof(HealthSystem), typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyDataSO _data;
    [SerializeField] private Animator _anim;

    [Header("Behaviour")]
    [SerializeField] private bool canMove = false;

    public Transform Player { get; private set; }
    public EnemyDataSO Data => _data;
    public EnemyAttackType AttackType => _data.attackType;
    public EnemyShoot Shoot { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public bool IsOnCooldown { get; set; } = false;
    public bool CanMove => canMove;

    private HealthSystem _healthSystem;
    private List<EnemyStates> _states = new();
    private EnemyStates _currentState;
    private Coroutine _cooldownCoroutine;
    private void Awake()
    {
        Shoot = GetComponent<EnemyShoot>();
        _healthSystem = GetComponent<HealthSystem>();
        Agent = GetComponent<NavMeshAgent>();
        SetupFSM();
    }
    private void OnEnable() => _healthSystem.onDie += OnDie_ChangeState;
    private void OnDisable() => _healthSystem.onDie -= OnDie_ChangeState;
    private void Update() => _currentState?.OnUpdate();

    public void Initialize(Transform player)
    {
        Player = player;
    }
    private void SetupFSM()
    {
        if (canMove)
        {
            _states.Add(new EnemyStateRoam());
            _states.Add(new EnemyStateFollow());
        }

        _states.Add(new EnemyStateIdle());
        _states.Add(new EnemyStateAttack());
        _states.Add(new EnemyStateDie());

        foreach (EnemyStates s in _states)
            s.Initialize(_anim, this);

        // Starts in roam or idle depending on if it can move
        _currentState = canMove
            ? FindState(StateTypeEnemy.Roam)
            : FindState(StateTypeEnemy.Idle);

        _currentState.OnEnter();
    }
    public void SwitchState(EnemyStates newState)
    {
        if (_currentState == newState) return;
        _currentState.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }
    public EnemyStates FindState(StateTypeEnemy type)
    {
        foreach (EnemyStates s in _states)
            if (s.state == type) return s;
        return null;
    }
    public bool CheckForNearPlayer()
    {
        if (Player == null) return false;
        return Vector3.Distance(transform.position, Player.position) < _data.detectionRange;
    }
    public bool CheckForAttackRange()
    {
        if (Player == null) return false;
        return Vector3.Distance(transform.position, Player.position) < _data.attackRange;
    }
    public void OnAttackCycleComplete()
    {
        IsOnCooldown = true;
        SwitchState(FindState(canMove ? StateTypeEnemy.Roam : StateTypeEnemy.Idle));
        if (_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        _cooldownCoroutine = StartCoroutine(AttackCooldown());
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_data.attackCooldown);
        IsOnCooldown = false;
        _cooldownCoroutine = null;
    }
    public Animator GetAnim() => _anim;
    private void OnDie_ChangeState() => SwitchState(FindState(StateTypeEnemy.Die));
}