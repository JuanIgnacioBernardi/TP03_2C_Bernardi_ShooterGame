using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EnemyShoot), typeof(HealthSystem))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyDataSO _data;
    [SerializeField] private Animator _anim;
    [SerializeField] private float attackCooldown = 2f;
    private Coroutine _cooldownCoroutine;

    public Transform Player { get; private set; }
    public EnemyDataSO Data => _data;
    public EnemyAttackType AttackType => _data.attackType;
    public EnemyShoot Shoot { get; private set; }

    private HealthSystem _healthSystem;
    private List<EnemyStates> _states = new();
    private EnemyStates _currentState;
    public bool IsOnCooldown { get; set; } = false;
    private void Awake()
    {
        Shoot = GetComponent<EnemyShoot>();
        _healthSystem = GetComponent<HealthSystem>();
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
        _states.Add(new EnemyStateIdle());
        _states.Add(new EnemyStateAttack());
        _states.Add(new EnemyStateDie());

        foreach (EnemyStates s in _states)
            s.Initialize(_anim, this);

        _currentState = FindState(StateTypeEnemy.Idle);
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
    public void OnAttackCycleComplete()
    {
        IsOnCooldown = true;
        SwitchState(FindState(StateTypeEnemy.Idle));
        if (_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        _cooldownCoroutine = StartCoroutine(AttackCooldown());
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        IsOnCooldown = false;
        _cooldownCoroutine = null;
    }
    public Animator GetAnim() => _anim;
    private void OnDie_ChangeState() => SwitchState(FindState(StateTypeEnemy.Die));
}