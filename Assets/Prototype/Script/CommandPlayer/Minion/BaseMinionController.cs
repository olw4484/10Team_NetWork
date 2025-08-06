using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(PhotonView))]
public abstract class BaseMinionController : MonoBehaviour, IDamageable, IPunInstantiateMagicCallback
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

    // 이동명령 관련
    private bool isAttackMove = false;
    private bool isMovingToPosition = false;
    private Vector3 attackMoveTarget;
    private Vector3 targetPosition;

    // 수동제어
    protected bool isManual = false;
    public virtual bool IsManualControl => isManual;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] {name} - OnPhotonInstantiate");

        object[] instantiationData = info.photonView.InstantiationData;
        if (instantiationData != null && instantiationData.Length > 2)
        {
            int minionType = (int)instantiationData[0];
            int teamId = (int)instantiationData[1];
            string groupId = (string)instantiationData[2];

            RpcInitialize(minionType, teamId, groupId);
        }
    }


    // --- 초기화 ---
    protected virtual void Awake()
    {
        photonView = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
        view = GetComponentInChildren<MinionView>();

        Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] {name} - BaseMinionController.Awake");
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

        Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] {name} - BaseMinionController.Start");
    }

    protected virtual void Update()
    {
        if (isDead || !photonView.IsMine) return;
        attackTimer += Time.deltaTime;

        // 공격 우선, 없으면 이동
        if (attackTarget != null)
        {
            HandleAttackTarget();
        }
        else if (moveTarget != null && !isAttackMove && !isMovingToPosition)
        {
            if (!agent.pathPending && agent.remainingDistance > agent.stoppingDistance)
                agent.SetDestination(moveTarget.position);
        }

        // 공격-이동 모드
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
    /// 공격 대상 처리
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

    // 타겟 setter들
    public void SetMoveTarget(Transform target) => moveTarget = target;
    public void SetAttackTarget(Transform target) => attackTarget = target;
    public void ClearTargets()
    {
        moveTarget = null;
        attackTarget = null;
    }

    // 공격이동/단일이동
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

    // 웨이포인트 이동
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
        Debug.Log($"[DEBUG] MoveToNextWaypoint 진입. isFollowingWaypoint={isFollowingWaypoint}, currentWaypointIndex={currentWaypointIndex}");
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

    // 외부제어
    public virtual void SetSelected(bool isSelected) { }
    public virtual void SetManualControl(bool isManual)
    {
        this.isManual = isManual;
    }

    // -------- 공격 처리 --------
    protected abstract void TryAttack();

    // 데미지 처리
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

    // RPC 처리
    #region RPC_CODE
    [PunRPC]
    public virtual void RpcInitialize(int minionType, int teamId, string groupId)
    {
        // 타입별 데이터 할당
        MinionDataSO data = MinionFactory.Instance.GetMinionData((MinionType)minionType);
        this.data = data;
        this.moveSpeed = data.moveSpeed;
        this.attackRange = data.attackRange;
        this.attackCooldown = data.attackCooldown;
        this.attackPower = data.attackPower;
        this.maxHP = data.maxHP;
        this.currentHP = maxHP;

        this.teamId = teamId;

        // 웨이포인트는 WaypointManager에서 가져오기 (싱글턴 관리)
        WaypointGroup wp = WaypointManager.Instance.GetWaypointGroup(groupId);
        this.waypointGroup = wp;

        this.currentWaypointIndex = 0;

        // 팀별 색상
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
