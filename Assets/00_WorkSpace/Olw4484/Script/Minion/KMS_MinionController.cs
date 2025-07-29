using Photon.Pun;
using System.Collections;
using System.Resources;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using static KMS_ISelectable;
using static KMS_ResourceSystem;

public class MinionController : MonoBehaviour, IDamageable , KMS_ISelectable
{
    [Header("Settings")]
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;
    public int attackPower;
    public int maxHP;
    private Vector3 attackMoveTarget;
    private float attackMoveStopDistance = 0.1f;
    public int teamId;

    public MinionView view;
    public MinionDataSO data;
    public LayerMask enemyLayerMask;
    private int currentHP;
    private Transform target;
    private Vector3 targetPosition;
    private float attackTimer = 0f;

    // 이동 그룹
    public KMS_WaypointGroup waypointGroup;
    private int currentWaypointIndex = 0;
    
    private Coroutine moveCoroutine;


    // 상태
    private bool isDead = false;
    private bool isAttackMove = false;
    private bool isMovingToPosition = false;
    public bool IsManual { get; private set; } = false;

    public PhotonView photonView;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        photonView = GetComponent<PhotonView>();
        view = GetComponentInChildren<MinionView>();
    }


    void Start()
    {
        currentHP = maxHP;
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.angularSpeed = 999f;
            agent.acceleration = 99f;
            agent.stoppingDistance = attackRange * 0.8f;
            agent.updateRotation = false;
        }
        if (waypointGroup != null && waypointGroup.GetWaypointCount() > 0)
        {
            MoveToNextWaypoint();
        }
    }

    void Update()
    {
        if (isDead) return;

        if (agent.velocity.sqrMagnitude > 0.01f)
            transform.forward = agent.velocity.normalized;

        if (isMovingToPosition)
        {
            // agent.pathPending: 경로 계산 중이면 true
            if (!agent.pathPending && agent.remainingDistance < 0.1f)
            {
                isMovingToPosition = false;
                agent.isStopped = true;
            }
        }

        // 공격이동
        if (isAttackMove)
        {
            var enemy = FindClosestEnemyInRange();
            if (enemy != null)
            {
                target = enemy.transform;
                isAttackMove = false;
            }
            else
            {
                // 목적지까지 이동
                if (agent.destination != attackMoveTarget)
                    agent.SetDestination(attackMoveTarget);

                if (!agent.pathPending && agent.remainingDistance < attackMoveStopDistance)
                {
                    isAttackMove = false;
                    agent.isStopped = true;
                }
            }
            return;
        }

        // 타겟 공격
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > attackRange)
            {
                if (agent.destination != target.position)
                    agent.SetDestination(target.position);
            }
            else
            {
                agent.isStopped = true;
                TryAttack();
            }
        }

        attackTimer += Time.deltaTime;

        if (waypointGroup != null && currentWaypointIndex > 0 && !agent.pathPending && agent.remainingDistance < 0.1f)
        {
            MoveToNextWaypoint();
        }
    }

    public void Initialize(MinionDataSO data, Transform target, KMS_WaypointGroup waypointGroup = null, int teamId = 0)
    {
        this.data = data;
        this.moveSpeed = data.moveSpeed;
        this.attackRange = data.attackRange;
        this.attackCooldown = data.attackCooldown;
        this.attackPower = data.attackPower;
        this.maxHP = data.maxHP;
        this.currentHP = maxHP;
        this.target = target;
        this.teamId = teamId;
        this.waypointGroup = waypointGroup;
        this.currentWaypointIndex = 0;

        if (this.waypointGroup != null && this.waypointGroup.GetWaypointCount() > 0)
        {
            var firstPoint = this.waypointGroup.GetWaypoint(0);
            if (firstPoint != null)
            {
                agent.isStopped = false;
                agent.SetDestination(firstPoint.position);
                currentWaypointIndex = 1; // 다음 이동 때 1번 인덱스부터
            }
        }
    }

    private void MoveToNextWaypoint()
    {
        if (waypointGroup == null) return;
        if (currentWaypointIndex >= waypointGroup.GetWaypointCount()) return;

        Transform nextPoint = waypointGroup.GetWaypoint(currentWaypointIndex);
        currentWaypointIndex++;

        if (nextPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(nextPoint.position);
        }
    }

    private Transform FindClosestEnemyInRange()
    {
        float searchRadius = attackRange * 1.5f; // 필요에따라 조절
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, enemyLayerMask);
        float minDist = float.MaxValue;
        Transform closest = null;

        foreach (var col in colliders)
        {
            var minion = col.GetComponent<MinionController>();
            if (minion != null && minion.teamId == this.teamId)
                continue; // 아군이면 패스

            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = col.transform;
            }
        }
        return closest;
    }

    public void MoveToPosition(Vector3 position)
    {
        target = null;
        isAttackMove = false;
        isMovingToPosition = true;
        targetPosition = position;
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public void SetAttackMoveTarget(Vector3 point)
    {
        target = null;
        isMovingToPosition = false;
        isAttackMove = true;
        attackMoveTarget = point;
        agent.isStopped = false;
        agent.SetDestination(point);
    }

    public void SetTarget(Transform newTarget)
    {
        isMovingToPosition = false;
        isAttackMove = false;
        target = newTarget;
        if (newTarget != null)
        {
            agent.isStopped = false;
            agent.SetDestination(newTarget.position);
        }
    }


    #region Attack
    // 공격 시도
    private void TryAttack()
    {
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            view?.PlayMinionAttackAnimation();
            // 데미지 처리
            var damageable = target?.GetComponent<IDamageable>();
            damageable?.TakeDamage(attackPower, gameObject);
        }
    }

    // 데미지 적용 // 애니메이션 클립 Attack 안에 이벤트 추가: ApplyDamage
    private void ApplyDamage()
    {
        if (target != null)
        {
            var damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(attackPower, gameObject);
        }
    }

    public void TakeDamage(int damage, GameObject attacker = null)
    {
        if (isDead) return;

        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die(attacker);
        }
    }
    #endregion

    private void Die(GameObject killer)
    {
        if (isDead) return;

        isDead = true;

        view?.PlayMinionDeathAnimation();

        if (EventManager.Instance != null)
        {
            EventManager.Instance.MinionDead(this, killer);
            EventManager.Instance.MinionKillConfirmed(killer, this);
        }

        // 삭제 시 동기화
        if (PhotonNetwork.InRoom)
            PhotonNetwork.Destroy(gameObject);
        else
            Destroy(gameObject, 1f);
    }

    public void SetManualControl(bool isManual)
    {
        IsManual = isManual;
    }

    public void SetSelected(bool isSelected)
    {
        view?.SetHighlight(isSelected);
    }

    #region KMS_ISelectable
    public void Select()
    {
        view?.SetHighlight(true);
    }

    public void Deselect()
    {
        view?.SetHighlight(false);
    }

    public SelectableType GetSelectableType() => SelectableType.Minion;
    #endregion
    #region RPC_Minion
    [PunRPC]
    public void RpcMoveToPosition(Vector3 position)
    {
        MoveToPosition(position);
    }

    [PunRPC]
    public void RpcSetTarget(int targetViewID)
    {
        var targetObj = PhotonView.Find(targetViewID);
        if (targetObj != null)
            SetTarget(targetObj.transform);
    }
    #endregion
}
