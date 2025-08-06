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

        if (isFollowingWaypoint)
        {
            Debug.Log($"[UpdateCheck] isFollowingWaypoint: TRUE, 현재 웨이포인트 인덱스: {currentWaypointIndex}");
        }
        else
        {
            Debug.Log($"[UpdateCheck] isFollowingWaypoint: FALSE");
        }

        // 최우선순위: 공격 중일 경우
        if (attackTarget != null)
        {
            Debug.Log("[UpdateCheck] 공격 대상 발견, 웨이포인트 로직 무시.");
            HandleRotation(); // 타겟 방향으로 회전
            HandleAttackTarget();
            return; // 공격 중이므로 다른 행동은 하지 않음
        }

        // 두 번째 우선순위: 수동 이동 명령이 있을 경우
        if (isAttackMove || isMovingToPosition)
        {
            Debug.Log("[UpdateCheck] 수동 이동 명령 실행, 웨이포인트 로직 무시.");
            HandleRotation(); // 이동 방향으로 회전
            HandleManualMove();
            return; // 수동 이동 중이므로 다른 행동은 하지 않음
        }

        // 가장 낮은 우선순위: 자동 웨이포인트 이동
        if (isFollowingWaypoint)
        {
            HandleRotation(); // 이동 방향으로 회전
            HandleWaypointMove();
        }
        else
        {
            // 아무것도 하지 않을 때, 에이전트 정지
            if (agent.hasPath)
            {
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
        Debug.Log("[FlagCheck] isFollowingWaypoint가 SetAttackMoveTarget에 의해 FALSE로 변경됨."); // 이 로그가 뜨는지 확인
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
        Debug.Log("[FlagCheck] isFollowingWaypoint가 MoveToPosition에 의해 FALSE로 변경됨."); // 이 로그가 뜨는지 확인
        targetPosition = position;
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    // 웨이포인트 이동
    protected IEnumerator WaitAndMoveToNextWaypoint()
    {
        waitingForNextWaypoint = true;
        Debug.Log($"[WaypointCheck] 코루틴 진입, 인덱스: {currentWaypointIndex}");
        yield return new WaitForSeconds(0.01f);
        currentWaypointIndex++;
        Debug.Log($"[WaypointCheck] 인덱스 증가: {currentWaypointIndex}");
        MoveToNextWaypoint();
        waitingForNextWaypoint = false;
    }
    protected void MoveToNextWaypoint()
    {
        Debug.Log($"[MoveToNextWaypoint] {name}: 인덱스 {currentWaypointIndex}, 그룹: {waypointGroup?.groupId}");
        if (waypointGroup == null) { Debug.LogError("[Minion] WaypointGroup == null"); return; }
        if (currentWaypointIndex >= waypointGroup.GetWaypointCount())
        {
            Debug.LogWarning("[Minion] currentWaypointIndex가 WaypointCount를 벗어남");
            return;
        }
        Transform next = waypointGroup.GetWaypoint(currentWaypointIndex);
        if (next != null)
        {
            Debug.Log($"[Minion] {name} → 다음 웨이포인트: {next.name} ({next.position})");
            agent.isStopped = false;
            agent.SetDestination(next.position);
        }
        else
        {
            Debug.LogWarning("[Minion] next Waypoint가 null임");
        }
    }
    private void HandleManualMove()
    {
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
    private void HandleWaypointMove()
    {
        if (!waitingForNextWaypoint && waypointGroup != null)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                if (currentWaypointIndex < waypointGroup.GetWaypointCount() - 1)
                {
                    StartCoroutine(WaitAndMoveToNextWaypoint());
                }
                else
                {
                    isFollowingWaypoint = false;
                    agent.isStopped = true;
                }
            }
        }
    }

    private void HandleRotation()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
        else if (attackTarget != null)
        {
            Vector3 direction = (attackTarget.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
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
