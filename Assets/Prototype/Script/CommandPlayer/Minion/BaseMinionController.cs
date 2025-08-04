using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(PhotonView))]
public abstract class BaseMinionController : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;
    public int attackPower;
    public int maxHP;
    protected int currentHP;

    [Header("Data")]
    public MinionDataSO data;

    [Header("Components")]
    public PhotonView photonView;
    protected NavMeshAgent agent;
    protected MinionView view;

    [Header("State")]
    protected Transform target;
    protected float attackTimer = 0f;
    protected bool isDead = false;
    public int teamId;

    // -------- WayPoint --------
    protected WaypointGroup waypointGroup;
    protected int currentWaypointIndex = 0;
    protected bool isFollowingWaypoint = false;
    protected bool waitingForNextWaypoint = false;

    // 
    protected bool IsManual = false;
    public virtual bool IsManualControlled => IsManual;

    private bool isAttackMove = false;
    private bool isMovingToPosition = false;

    private Vector3 attackMoveTarget;
    private Vector3 targetPosition;

    // -------- 초기화 --------
    protected virtual void Awake()
    {
        photonView = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
        view = GetComponentInChildren<MinionView>();
    }

    protected virtual void Start()
    {
        currentHP = maxHP;

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = attackRange * 0.8f;
            agent.angularSpeed = 999f;
            agent.acceleration = 99f;
            agent.updateRotation = false;
        }

        currentWaypointIndex = 0;
        isFollowingWaypoint = false;
    }

    public virtual void Initialize(MinionDataSO data, Transform target = null, int teamId = 0)
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
    }

    // -------- Update는 필요 시 오버라이드 --------
    protected virtual void Update()
    {
        if (isDead) return;
        attackTimer += Time.deltaTime;
        if (!photonView.IsMine) return;

        // 수동 제어 로직
        if (isAttackMove && target == null)
        {
            agent.SetDestination(attackMoveTarget);
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                isAttackMove = false;
                agent.isStopped = true;
            }
        }
        else if (isMovingToPosition)
        {
            agent.SetDestination(targetPosition);
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                isMovingToPosition = false;
                agent.isStopped = true;
            }
        }
        HandleTarget(); // 타겟 존재 시 전투 로직
    }

    protected virtual void HandleTarget()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
            TryAttack();
        }

        isFollowingWaypoint = false;
    }

    protected IEnumerator WaitAndMoveToNextWaypoint()
    {
        waitingForNextWaypoint = true;
        yield return new WaitForSeconds(0.01f);
        currentWaypointIndex++;
        MoveToNextWaypoint();
        waitingForNextWaypoint = false;
    }

    protected void ResumeWaypointMove()
    {
        int nearest = FindNearestWaypointIndex();
        float threshold = agent.stoppingDistance;
        var point = waypointGroup.GetWaypoint(nearest);

        if (point != null && Vector3.Distance(transform.position, point.position) < threshold)
            nearest++;

        currentWaypointIndex = nearest;
        isFollowingWaypoint = true;
        MoveToNextWaypoint();
    }

    protected void MoveToNextWaypoint()
    {
        if (waypointGroup == null) return;
        if (currentWaypointIndex >= waypointGroup.GetWaypointCount()) return;

        Transform next = waypointGroup.GetWaypoint(currentWaypointIndex);
        if (next != null)
        {
            agent.isStopped = false;
            agent.SetDestination(next.position);
        }
    }

    public void SetAttackMoveTarget(Vector3 point)
    {
        isAttackMove = true;
        isMovingToPosition = false;
        target = null;
        isFollowingWaypoint = false;

        attackMoveTarget = point;
        agent.isStopped = false;
        agent.SetDestination(point);
    }

    public void MoveToPosition(Vector3 position)
    {
        isAttackMove = false;
        isMovingToPosition = true;
        target = null;
        isFollowingWaypoint = false;

        targetPosition = position;
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    protected int FindNearestWaypointIndex()
    {
        if (waypointGroup == null) return 0;
        int nearest = 0;
        float minDist = float.MaxValue;

        for (int i = 0; i < waypointGroup.GetWaypointCount(); i++)
        {
            float dist = Vector3.Distance(transform.position, waypointGroup.GetWaypoint(i).position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = i;
            }
        }

        return nearest;
    }
    public virtual void SetWaypointGroup(WaypointGroup group)
    {
        waypointGroup = group;
    }

    public virtual void SetSelected(bool isSelected) { }
    public virtual void SetManualControl(bool isManual)
    {

    }
    public virtual void Initialize(MinionDataSO data, Transform target, WaypointGroup waypointGroup = null, int teamId = 0)
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
    }

    // -------- 공격 처리 --------
    protected abstract void TryAttack();

    public virtual void SetTarget(Transform newTarget)
    {
        target = newTarget;
        isFollowingWaypoint = false;

        if (target != null)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    public virtual void TakeDamage(int damage, GameObject attacker = null)
    {
        if (isDead) return;

        currentHP -= damage;
        if (currentHP <= 0)
            Die(attacker);
    }

    protected virtual void Die(GameObject killer)
    {
        if (isDead) return;
        isDead = true;
        view?.PlayMinionDeathAnimation();

        if (EventManager.Instance != null)
        {
            EventManager.Instance.MinionDead(this as MinionController, killer);
            EventManager.Instance.MinionKillConfirmed(killer, this as MinionController);
        }

        if (PhotonNetwork.InRoom)
            PhotonNetwork.Destroy(gameObject);
        else
            Destroy(gameObject, 1f);
    }
    #region RPC_CODE
    [PunRPC]
    public void RPC_TakeDamage(int damage, int attackerViewID)
    {
        if (isDead) return;
        currentHP -= damage;

        if (currentHP <= 0)
        {
            GameObject killer = null;
            var attackerPV = PhotonView.Find(attackerViewID);
            if (attackerPV != null)
                killer = attackerPV.gameObject;

            Die(killer);
        }
    }
    #endregion
}
