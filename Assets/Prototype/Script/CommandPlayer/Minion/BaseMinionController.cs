using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(PhotonView))]
public abstract class BaseMinionController : MonoBehaviour, IDamageable, IPunInstantiateMagicCallback
{
    [Header("Stats")]
    public float moveSpeed;
    public float attackRange;
    public float detectRange;
    public float attackCooldown;
    public int attackPower;
    public int maxHP;
    protected int currentHP;

    [Header("Data")]
    public MinionDataSO data;
    public LayerMask targetMask;

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
    public float waypointStoppingDistance = 0.8f;
    private float searchInterval = 0.2f;
    private float searchTimer = 0f;
    private Quaternion lastSentRotation;
    protected Animator animator;
    int IDamageable.teamId => this.teamId;
    bool IDamageable.isDead => this.isDead;

    [Header("TeamColor")]
    [SerializeField] private GameObject redBody;
    [SerializeField] private GameObject blueBody;

    // Waypoint & FSM
    protected WaypointGroup waypointGroup;
    protected int currentWaypointIndex = 0;
    protected bool isFollowingWaypoint = false;


    // 이동명령 관련
    private bool isAttackMove = false;
    private bool isMovingToPosition = false;
    private Vector3 attackMoveTarget;
    private Vector3 targetPosition;
    public bool isMoving = false;

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

            Rpc_Initialize(minionType, teamId, groupId);
        }
    }

    // --- 초기화 ---
    protected virtual void Awake()
    {
        photonView = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
        view = GetComponentInChildren<MinionView>();
        animator = GetComponentInChildren<Animator>();

        Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] {name} - BaseMinionController.Awake");
    }

    protected virtual void Start()
    {
        currentHP = maxHP;
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = waypointStoppingDistance;
            agent.angularSpeed = 999f;
            agent.acceleration = 99f;
            agent.updateRotation = false;
        }
        currentWaypointIndex = 0;

        Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] {name} - BaseMinionController.Start");
    }

    protected virtual void Update()
    {
        if (isDead) return;

        // 마스터 클라이언트에서만 실행되는 AI 로직
        if (PhotonNetwork.IsMasterClient)
        {
            attackTimer += Time.deltaTime;
            if (attackTarget == null || attackTarget.GetComponent<BaseMinionController>()?.isDead == true)
            {
                searchTimer += Time.deltaTime;
                if (searchTimer >= searchInterval)
                {
                    SearchForTarget();
                    searchTimer = 0f;
                }
            }
        }

        // 최우선순위: 공격 중
        if (attackTarget != null)
        {
            HandleRotation();
            HandleAttackTarget();
            return;
        }

        // 두 번째 우선순위: 수동 이동 명령
        if (isAttackMove || isMovingToPosition)
        {
            HandleRotation();
            HandleManualMove();
            return;
        }

        // 가장 낮은 우선순위: 자동 웨이포인트 이동
        if (isFollowingWaypoint)
        {
            HandleRotation();
            HandleWaypointMove();
        }
        else
        {
            if (agent.hasPath)
            {
                agent.isStopped = true;
            }
        }

        if (attackTarget == null && !isDead && !isFollowingWaypoint)
        {
            isFollowingWaypoint = true;
            MoveToNextWaypoint();
        }

        // --- RPC를 사용한 애니메이션 동기화 로직 ---
        if (photonView.IsMine)
        {
            bool currentIsMovingState = animator.GetBool("IsMoving");
            bool newIsMovingState = agent.velocity.magnitude > 0.1f && !agent.isStopped;

            if (currentIsMovingState != newIsMovingState)
            {
                // 상태가 변경되었을 때만 모든 클라이언트에 RPC 호출
                photonView.RPC(nameof(RPC_SetMoving), RpcTarget.All, newIsMovingState);
            }
        }
    }

    /// <summary>
    /// 공격 대상 처리
    /// </summary>
    protected virtual void HandleAttackTarget()
    {
        Debug.Log($"[{photonView.name}] HandleAttackTarget() called!");
        if (attackTarget == null)
        {
            Debug.Log("attackTarget is null!");
            return;
        }
        float distance = Vector3.Distance(transform.position, attackTarget.position);
        Debug.Log($"[AttackCheck] distance={distance}, attackRange={attackRange}");
        if (distance <= attackRange)
        {
            Debug.Log($"In attack range, [{photonView.name}]calling TryAttack()");
            agent.isStopped = true;
            agent.ResetPath();
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
        Debug.Log("[FlagCheck] isFollowingWaypoint가 SetAttackMoveTarget에 의해 FALSE로 변경됨.");
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
        Debug.Log("[FlagCheck] isFollowingWaypoint가 MoveToPosition에 의해 FALSE로 변경됨.");
        targetPosition = position;
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    protected void MoveToNextWaypoint()
    {
        if (waypointGroup == null)
        {
            Debug.LogError("[Minion] WaypointGroup == null");
            return;
        }
        if (currentWaypointIndex >= waypointGroup.GetWaypointCount())
        {
            Debug.LogWarning("[Minion] currentWaypointIndex가 WaypointCount를 벗어남");
            return;
        }

        Transform next = waypointGroup.GetWaypoint(currentWaypointIndex);
        if (next != null)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("Rpc_SetDestination", RpcTarget.All, next.position);
            }
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
        if (!PhotonNetwork.IsMasterClient) return;

        if (waypointGroup != null)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.4f)
            {
                if (currentWaypointIndex < waypointGroup.GetWaypointCount() - 1)
                {
                    currentWaypointIndex++;
                    MoveToNextWaypoint();
                }
                else
                {
                    isFollowingWaypoint = false;
                    agent.isStopped = true;
                    agent.ResetPath();
                    Debug.Log("[Waypoint] 마지막 웨이포인트 도달 완료");
                }
            }
        }
    }


    private void HandleRotation()
    {
        if (!photonView.IsMine) return;

        Quaternion targetRotation = transform.rotation;

        if (agent.velocity.sqrMagnitude > 0.1f)
            targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        else if (attackTarget != null)
        {
            Vector3 direction = (attackTarget.position - transform.position).normalized;
            if (direction != Vector3.zero)
                targetRotation = Quaternion.LookRotation(direction);
        }

        // 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

        // 네트워크 전파 (예: 5도 이상 각도 차이 있을 때만)
        if (Quaternion.Angle(lastSentRotation, transform.rotation) > 5f)
        {
            photonView.RPC("RPC_SyncRotation", RpcTarget.Others, transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            lastSentRotation = transform.rotation;
        }
    }

    private void SearchForTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, targetMask);

        foreach (var hit in hits)
        {
            var target = hit.GetComponent<IDamageable>();
            Debug.Log($"[전체로그] hit: {hit.name} | {target?.GetType().Name} | 팀: {target?.teamId} | 죽음: {target?.isDead}");

            if (target != null && target.teamId != this.teamId && !target.isDead)
            {
                Debug.Log($"[타겟선정] 공격대상: {hit.name} | {target.GetType().Name}");
                attackTarget = ((MonoBehaviour)target).transform;
                break;
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

    public void OnAttackAnimationEvent()
    {
        if (!PhotonNetwork.IsMasterClient || attackTarget == null || isDead) return;

        var targetPV = attackTarget.GetComponent<PhotonView>();
        if (targetPV == null) return;

        photonView.RPC(nameof(RPC_DealDamage), RpcTarget.All, targetPV.ViewID, attackPower);
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
    public virtual void Rpc_Initialize(int minionType, int teamId, string groupId)
    {
        // 타입별 데이터 할당
        MinionDataSO data = MinionFactory.Instance.GetMinionData((MinionType)minionType);
        this.data = data;
        this.moveSpeed = data.moveSpeed;
        this.attackRange = data.attackRange;
        this.detectRange = data.detectRange;
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

        Debug.Log($"[InitCheck] waypointGroup: {(waypointGroup != null ? "존재함" : "Null")}, IsManualControl: {IsManualControl}");

        if (waypointGroup != null && !IsManualControl)
        {
            isFollowingWaypoint = true;
            Debug.Log("[InitCheck] isFollowingWaypoint를 TRUE로 설정. 웨이포인트 이동 시작.");
            MoveToNextWaypoint();
        }
        else
        {
            isFollowingWaypoint = false;
            Debug.Log("[InitCheck] 조건 불충분으로 isFollowingWaypoint가 FALSE로 남음.");
        }
    }

    [PunRPC]
    public void Rpc_SetDestination(Vector3 destination)
    {
        Debug.Log($"[RPC] Rpc_SetDestination 호출 on {gameObject.name} : {destination}, agent.enabled={agent.enabled}, speed={agent.speed}");
        if (agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(destination);
        }
    }
    [PunRPC]
    public void RPC_TakeDamage(int damage, int attackerViewID)
    {
        GameObject killer = null;
        var attackerPV = PhotonView.Find(attackerViewID);
        if (attackerPV != null)
            killer = attackerPV.gameObject;
        TakeDamage(damage, killer);
    }

    [PunRPC]
    public void RPC_DealDamage(int targetViewID, int dmg)
    {
        var targetPV = PhotonView.Find(targetViewID);
        if (targetPV != null)
        {
            var damageable = targetPV.GetComponent<IDamageable>();
            damageable?.TakeDamage(dmg, this.gameObject);
        }
    }

    [PunRPC]
    public void RPC_SyncRotation(float x, float y, float z, float w)
    {
        transform.rotation = new Quaternion(x, y, z, w);
    }

    [PunRPC]
    private void RPC_SetMoving(bool isMoving)
    {
        //animator.SetBool("IsMoving", isMoving);
        Debug.Log("Movement state received: " + isMoving);
    }
    #endregion
}