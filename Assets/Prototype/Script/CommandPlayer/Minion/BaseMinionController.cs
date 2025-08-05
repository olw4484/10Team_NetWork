using Photon.Pun;
using System.Collections;
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
    protected Transform moveTarget;
    protected Transform attackTarget;
    protected float attackTimer = 0f;
    protected bool isDead = false;
    public int teamId;

    [Header("TeamColor")]
    [SerializeField] private GameObject redBody;
    [SerializeField] private GameObject blueBody;

    // Waypoint & FSM
    protected WaypointGroup waypointGroup;
    protected int currentWaypointIndex = 0;
    protected bool isFollowingWaypoint = false;
    protected bool waitingForNextWaypoint = false;

    // �̵���� ����
    private bool isAttackMove = false;
    private bool isMovingToPosition = false;
    private Vector3 attackMoveTarget;
    private Vector3 targetPosition;

    // ��������
    protected bool isManual = false;
    public virtual bool IsManualControl => isManual;

    // --- �ʱ�ȭ ---
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

    /// <summary>
    /// Minion �⺻ ����
    /// </summary>
    public virtual void LocalInitialize(
        MinionDataSO data,
        Transform moveTarget = null,
        Transform attackTarget = null,
        WaypointGroup waypointGroup = null,
        int teamId = 0)
    {
        Debug.Log("[DEBUG] Initialize() ����");
        this.data = data;
        this.moveSpeed = data.moveSpeed;
        this.attackRange = data.attackRange;
        this.attackCooldown = data.attackCooldown;
        this.attackPower = data.attackPower;
        this.maxHP = data.maxHP;
        this.currentHP = maxHP;

        this.moveTarget = moveTarget;
        this.attackTarget = attackTarget;
        this.teamId = teamId;
        this.waypointGroup = waypointGroup;
        Debug.Log($"[DEBUG] waypointGroup is {(waypointGroup != null ? "NOT NULL" : "NULL")}");
        Debug.Log($"[Minion] WaypointGroup injected? {(waypointGroup != null)} / MoveTarget: {moveTarget} / AttackTarget: {attackTarget}");
        this.currentWaypointIndex = 0;

        // ���� ����
        if (redBody != null) redBody.SetActive(teamId == 0);
        if (blueBody != null) blueBody.SetActive(teamId == 1);

        if (waypointGroup != null && IsManualControl == false)
        {
            isFollowingWaypoint = true;
            MoveToNextWaypoint();
        }
    }

    protected virtual void Update()
    {
        if (isDead || !photonView.IsMine) return;
        attackTimer += Time.deltaTime;

        // ���� �켱, ������ �̵�
        if (attackTarget != null)
        {
            HandleAttackTarget();
        }
        else if (moveTarget != null && !isAttackMove && !isMovingToPosition)
        {
            if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
                agent.SetDestination(moveTarget.position);
        }

        // ����-�̵� ���
        if (isAttackMove && attackTarget == null)
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
    }

    /// <summary>
    /// ���� ��� ó��
    /// </summary>
    protected virtual void HandleAttackTarget()
    {
        float distance = Vector3.Distance(transform.position, attackTarget.position);
        if (distance <= attackRange)
        {
            agent.isStopped = true;
            TryAttack();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(attackTarget.position);
        }
    }

    // Ÿ�� setter��
    public void SetMoveTarget(Transform target) => moveTarget = target;
    public void SetAttackTarget(Transform target) => attackTarget = target;
    public void ClearTargets()
    {
        moveTarget = null;
        attackTarget = null;
    }

    // �����̵�/�����̵�
    public void SetAttackMoveTarget(Vector3 point)
    {
        isAttackMove = true;
        isMovingToPosition = false;
        ClearTargets();
        isFollowingWaypoint = false;
        attackMoveTarget = point;
        agent.isStopped = false;
        agent.SetDestination(point);
    }
    public void MoveToPosition(Vector3 position)
    {
        isAttackMove = false;
        isMovingToPosition = true;
        ClearTargets();
        isFollowingWaypoint = false;
        targetPosition = position;
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    // ��������Ʈ �̵�
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
        Debug.Log($"[DEBUG] MoveToNextWaypoint ����. isFollowingWaypoint={isFollowingWaypoint}, currentWaypointIndex={currentWaypointIndex}");
        if (waypointGroup == null) return;
        if (currentWaypointIndex >= waypointGroup.GetWaypointCount()) return;
        Transform next = waypointGroup.GetWaypoint(currentWaypointIndex);
        if (next != null)
        {
            agent.isStopped = false;
            agent.SetDestination(next.position);
        }
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
    public virtual void SetWaypointGroup(WaypointGroup group) => waypointGroup = group;

    // �ܺ�����
    public virtual void SetSelected(bool isSelected) { }
    public virtual void SetManualControl(bool isManual)
    {
        this.isManual = isManual;
    }

    // -------- ���� ó�� --------
    protected abstract void TryAttack();

    // ������ ó��
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

    // RPC ó��
    #region RPC_CODE
    [PunRPC]
    public virtual void RpcInitialize(int minionType, int teamId, int waypointIndex)
    {
        // Ÿ�Ժ� ������ �Ҵ�
        MinionDataSO data = MinionFactory.Instance.GetMinionData((MinionType)minionType);
        this.data = data;
        this.moveSpeed = data.moveSpeed;
        this.attackRange = data.attackRange;
        this.attackCooldown = data.attackCooldown;
        this.attackPower = data.attackPower;
        this.maxHP = data.maxHP;
        this.currentHP = maxHP;

        this.teamId = teamId;

        // ��������Ʈ�� WaypointManager���� �������� (�̱��� ����)
        WaypointGroup wp = WaypointManager.Instance.GetGroupByIndex(waypointIndex);
        this.waypointGroup = wp;

        this.currentWaypointIndex = 0;

        // ���� ����
        if (redBody != null) redBody.SetActive(teamId == 0);
        if (blueBody != null) blueBody.SetActive(teamId == 1);

        if (waypointGroup != null && !IsManualControl)
        {
            isFollowingWaypoint = true;
            MoveToNextWaypoint();
        }
    }

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
