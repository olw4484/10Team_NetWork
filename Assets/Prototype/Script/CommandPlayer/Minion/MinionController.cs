using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static ISelectable;

public class MinionController : MonoBehaviour, IDamageable, ISelectable
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
    public WaypointGroup waypointGroup;
    private int currentWaypointIndex = 0;

    // 상태
    private bool isDead = false;
    private bool isAttackMove = false;
    private bool isMovingToPosition = false;
    public bool IsManual { get; private set; } = false;
    private bool waitingForNextWaypoint = false;
    private bool isFollowingWaypoint = false;

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
        currentWaypointIndex = 0;
        isFollowingWaypoint = false; // 최초엔 false
    }

    void Update()
    {
        if (isDead) return;

        if (photonView.IsMine)
        {

            // 미니언 이동 방향 갱신
            if (agent.velocity.sqrMagnitude > 0.01f)
                transform.forward = agent.velocity.normalized;

            // --- 웨이포인트 재합류/최초 시작 ---
            if (!isFollowingWaypoint && waypointGroup != null && waypointGroup.GetWaypointCount() > 0)
            {
                ResumeWaypointMove();
            }

            // --- 웨이포인트 도착 판정 및 이동 ---
            if (isFollowingWaypoint && waypointGroup != null && currentWaypointIndex >= 0
                 && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !waitingForNextWaypoint)
            {
                StartCoroutine(WaitAndMoveToNextWaypoint());
            }


            // --- 공격이동 ---
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

            // --- 타겟 공격 ---
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
                // 웨이포인트 중단
                isFollowingWaypoint = false;
            }
            else
            {
                // 타겟이 완전히 사라졌고, 수동/공격이동도 아니면 라인 복귀
                if (!isAttackMove && !isMovingToPosition && !waitingForNextWaypoint)
                {
                    isFollowingWaypoint = false;
                }
            }
        }
        attackTimer += Time.deltaTime;
    }

    // --- 웨이포인트에서 가장 가까운 인덱스 찾기 ---
    private int FindNearestWaypointIndex()
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

    // --- 라인 복귀/최초 합류 ---
    private void ResumeWaypointMove()
    {
        int startIdx = currentWaypointIndex; // 현재 미니언의 waypointGroup상의 인덱스를 가져옵니다.
        if (startIdx < 0) startIdx = 0; // 혹시라도 음수값이 되지 않도록 방어 코드 추가

        int nearest = startIdx;
        float minDist = float.MaxValue;

        for (int i = startIdx; i < waypointGroup.GetWaypointCount(); i++)
        {
            var point = waypointGroup.GetWaypoint(i);
            if (point == null) continue;
            float dist = Vector3.Distance(transform.position, point.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = i;
            }
        }

        // 만약 내가 이미 nearest 웨이포인트에 충분히 가까우면 다음 인덱스로 넘긴다
        float arrivalThreshold = agent.stoppingDistance;
        Transform nearestPoint = waypointGroup.GetWaypoint(nearest);
        if (nearestPoint != null &&
            Vector3.Distance(transform.position, nearestPoint.position) < arrivalThreshold &&
            nearest + 1 < waypointGroup.GetWaypointCount())
        {
            nearest++;
        }

        currentWaypointIndex = nearest; // 미니언이 이동할 첫 웨이포인트 인덱스 설정
        isFollowingWaypoint = true;

        // --- 여기에서 바로 첫 이동 명령을 내립니다 ---
        MoveToNextWaypoint(); // 설정된 currentWaypointIndex로 이동 시작

        Debug.Log($"[Minion {gameObject.name}] Resumed Waypoint Move. Initial Destination Set to Waypoint Index: {currentWaypointIndex}");
    }

    private void MoveToNextWaypoint()
    {
        if (waypointGroup == null) return;
        if (currentWaypointIndex >= waypointGroup.GetWaypointCount()) return;

        Transform nextPoint = waypointGroup.GetWaypoint(currentWaypointIndex);
        Debug.Log($"[MoveToNextWaypoint] minion={gameObject.name}, index={currentWaypointIndex}, agentPos={transform.position}");

        if (nextPoint != null)
        {
            agent.isStopped = false;
            agent.SetDestination(nextPoint.position);
            Debug.Log($"[SetDestination] index={currentWaypointIndex}, dest=({nextPoint.position})");
        }
    }

    private IEnumerator WaitAndMoveToNextWaypoint()
    {
        waitingForNextWaypoint = true;
        yield return new WaitForSeconds(0.01f);
        currentWaypointIndex++;
        MoveToNextWaypoint();
        waitingForNextWaypoint = false;
    }

    private Transform FindClosestEnemyInRange()
    {
        float searchRadius = attackRange * 1.5f;
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, enemyLayerMask);
        float minDist = float.MaxValue;
        Transform closest = null;

        foreach (var col in colliders)
        {
            var minion = col.GetComponent<MinionController>();
            if (minion != null && minion.teamId == this.teamId)
                continue; // 아군 패스

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
        isFollowingWaypoint = false; // 수동 이동 명령 시 라인 이탈
        targetPosition = position;
        //agent.isStopped = false;
        //agent.SetDestination(position);
    }

    public void SetAttackMoveTarget(Vector3 point)
    {
        target = null;
        isMovingToPosition = false;
        isAttackMove = true;
        isFollowingWaypoint = false; // 공격이동 시 라인 이탈
        attackMoveTarget = point;
        agent.isStopped = false;
        agent.SetDestination(point);
    }

    public void SetTarget(Transform newTarget)
    {
        isMovingToPosition = false;
        isAttackMove = false;
        target = newTarget;
        isFollowingWaypoint = false; // 적 타겟 지정 시 라인 이탈
        if (newTarget != null)
        {
            agent.isStopped = false;
            agent.SetDestination(newTarget.position);
        }
    }

    // --- 공격/데미지 처리 --- 
    private void TryAttack()
    {
        if (!photonView.IsMine)
            return;
        if (attackTimer >= attackCooldown && target != null)
        {
            attackTimer = 0f;
            int targetViewID = target.GetComponent<PhotonView>().ViewID;
            photonView.RPC("RPC_TryAttack", RpcTarget.All, targetViewID);
        }
    }

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
            Die(attacker);
    }

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
        if (PhotonNetwork.InRoom)
            PhotonNetwork.Destroy(gameObject);
        else
            Destroy(gameObject, 1f);
    }

    public void SetManualControl(bool isManual) => IsManual = isManual;
    public void SetSelected(bool isSelected) => view?.SetHighlight(isSelected);

    #region ISelectable
    public void Select() => view?.SetHighlight(true);
    public void Deselect() => view?.SetHighlight(false);
    public SelectableType GetSelectableType() => SelectableType.Minion;
    #endregion

    #region RPC_Minion
    [PunRPC]
    public void RpcMoveToPosition(Vector3 position)
    {
        if (photonView.IsMine) 
        {
            MoveToPosition(position); 
            agent.isStopped = false;
            agent.SetDestination(position);
        }
    }
    [PunRPC]
    public void RpcSetTarget(int targetViewID)
    {
        if (!photonView.IsMine) return;

        var targetObj = PhotonView.Find(targetViewID);
        if (targetObj != null)
        {
            target = targetObj.transform;
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            target = null;
            // 필요시 ResumeWaypointMove();
        }
        isMovingToPosition = false;
        isAttackMove = false;
        isFollowingWaypoint = false;
    }
    [PunRPC]
    public void RpcSetAttackMoveTarget(Vector3 point)
    {
        if (photonView.IsMine) 
        {
            SetAttackMoveTarget(point); 
        }
    }
    #endregion

    #region RPC_Attack
    [PunRPC]
    void RPC_TryAttack(int targetViewID)
    {
        view?.PlayMinionAttackAnimation();
        var targetPV = PhotonView.Find(targetViewID);
        if (targetPV != null)
            targetPV.RPC("RPC_TakeDamage", RpcTarget.All, attackPower, photonView.ViewID);
    }
    [PunRPC]
    void RPC_TakeDamage(int damage, int attackerViewID)
    {
        if (isDead) return;
        currentHP -= damage;
        if (currentHP <= 0)
            photonView.RPC("RPC_Die", RpcTarget.All, attackerViewID);
    }
    [PunRPC]
    void RPC_Die(int attackerViewID)
    {
        if (isDead) return;
        isDead = true;
        view?.PlayMinionDeathAnimation(); // 애니메이션은 모든 클라이언트에서 실행

        // 중요: 마스터 클라이언트만 게임 상태 관련 이벤트를 발생시키고 동기화
        if (PhotonNetwork.IsMasterClient && EventManager.Instance != null)
        {
            var killerPV = PhotonView.Find(attackerViewID);
            GameObject killerObject = (killerPV != null) ? killerPV.gameObject : null;
            EventManager.Instance.MinionDead(this, killerObject);
            EventManager.Instance.MinionKillConfirmed(killerObject, this);
        }

        if (PhotonNetwork.InRoom)
            PhotonNetwork.Destroy(gameObject);
        else
            Destroy(gameObject, 1f);
    }
    #endregion

    public void Initialize(MinionDataSO data, Transform target, WaypointGroup waypointGroup = null, int teamId = 0)
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
        this.isFollowingWaypoint = false;
    }
}
