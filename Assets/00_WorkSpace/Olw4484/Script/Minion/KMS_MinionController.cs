using Photon.Pun;
using System.Collections;
using System.Resources;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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

    public MinionView view;
    public MinionDataSO data;
    public LayerMask enemyLayerMask;
    private int currentHP;
    private Transform target;
    private Vector3 targetPosition;
    private float attackTimer = 0f;

    public KMS_WaypointGroup waypointGroup;
    private int currentWaypointIndex = 0;
    
    private Coroutine moveCoroutine;


    // 상태
    private bool isDead = false;
    private bool isAttackMove = false;
    private bool isMovingToPosition = false;
    public bool IsManual { get; private set; } = false;

    public PhotonView photonView;


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        view = GetComponentInChildren<MinionView>();
    }


    void Start()
    {
        currentHP = maxHP;

        if (waypointGroup != null && waypointGroup.GetWaypointCount() > 0)
        {
            MoveToNextWaypoint();
        }
    }

    void Update()
    {
        if (isDead) return;

        if (isMovingToPosition)
        {
            MoveTowards(targetPosition);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                isMovingToPosition = false;
        }

        // 공격 이동 중이면
        if (isAttackMove)
        {
            // 주변 적 탐색
            var enemy = FindClosestEnemyInRange();
            if (enemy != null)
            {
                target = enemy.transform;
                isAttackMove = false; // 공격 상태로 전환
            }
            else
            {
                // 목적지로 이동
                MoveTowards(attackMoveTarget);
                if (Vector3.Distance(transform.position, attackMoveTarget) < attackMoveStopDistance)
                {
                    isAttackMove = false; // 도착하면 종료
                }
            }
            return; // 공격이동 중에는 아래 일반 AI 생략
        }

        // 기존 AI 로직 (타겟 지정 등)
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance > attackRange)
                MoveTowards(target.position);
            else
                TryAttack();
        }

        attackTimer += Time.deltaTime;
    }

    public void Initialize(MinionDataSO data, Transform target, KMS_WaypointGroup waypointGroup = null)
    {
        this.data = data;
        this.moveSpeed = data.moveSpeed;
        this.attackRange = data.attackRange;
        this.attackCooldown = data.attackCooldown;
        this.attackPower = data.attackPower;
        this.maxHP = data.maxHP;
        this.currentHP = maxHP;
        this.target = target;

        this.waypointGroup = waypointGroup;
        this.currentWaypointIndex = 0;

        if (target != null)
            StartCoroutine(MoveToTarget());
    }

    private void MoveTowards(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    private IEnumerator MoveTo(Transform point)
    {
        while (point != null && Vector3.Distance(transform.position, point.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, point.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        MoveToNextWaypoint(); // 다음 경로로
    }

    private void MoveToNextWaypoint()
    {
        if (currentWaypointIndex >= waypointGroup.GetWaypointCount()) return;

        Transform nextPoint = waypointGroup.GetWaypoint(currentWaypointIndex);
        currentWaypointIndex++;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveTo(nextPoint));
    }

    private Transform FindClosestEnemyInRange()
    {
        float searchRadius = attackRange * 1.5f; // 필요에 따라 조절
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, enemyLayerMask);
        float minDist = float.MaxValue;
        Transform closest = null;

        foreach (var col in colliders)
        {
            if (col.gameObject == gameObject) continue;
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = col.transform;
            }
        }
        return closest;
    }


    private IEnumerator MoveToTarget()
    {
        while (target != null && Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        moveCoroutine = null;
    }

    public void MoveToPosition(Vector3 position)
    {
        target = null;
        isAttackMove = false;
        isMovingToPosition = true;
        targetPosition = position;
    }

    public void SetAttackMoveTarget(Vector3 point)
    {
        target = null;
        isMovingToPosition = false;
        isAttackMove = true;
        attackMoveTarget = point;
    }

    public void SetTarget(Transform newTarget)
    {
        isMovingToPosition = false;
        isAttackMove = false;
        target = newTarget;
    }


    #region Attack
    // 공격 시도
    private void TryAttack()
    {
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            if (view != null && view.Animator != null)
                //view.Animator.SetFloat("AttackSpeed", data.AttackAnimSpeed); //공격속도가 필요하면 주석 해제하고 사용 가능

            view.PlayMinionAttackAnimation();
            // 공격 데미지 처리는 애니메이션 이벤트에서
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

    public SelectableType GetSelectableType() => SelectableType.Unit;
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
