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


    // ����
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

        // ���� �̵� ���̸�
        if (isAttackMove)
        {
            // �ֺ� �� Ž��
            var enemy = FindClosestEnemyInRange();
            if (enemy != null)
            {
                target = enemy.transform;
                isAttackMove = false; // ���� ���·� ��ȯ
            }
            else
            {
                // �������� �̵�
                MoveTowards(attackMoveTarget);
                if (Vector3.Distance(transform.position, attackMoveTarget) < attackMoveStopDistance)
                {
                    isAttackMove = false; // �����ϸ� ����
                }
            }
            return; // �����̵� �߿��� �Ʒ� �Ϲ� AI ����
        }

        // ���� AI ���� (Ÿ�� ���� ��)
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

        MoveToNextWaypoint(); // ���� ��η�
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
        float searchRadius = attackRange * 1.5f; // �ʿ信 ���� ����
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
    // ���� �õ�
    private void TryAttack()
    {
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            if (view != null && view.Animator != null)
                //view.Animator.SetFloat("AttackSpeed", data.AttackAnimSpeed); //���ݼӵ��� �ʿ��ϸ� �ּ� �����ϰ� ��� ����

            view.PlayMinionAttackAnimation();
            // ���� ������ ó���� �ִϸ��̼� �̺�Ʈ����
        }
    }

    // ������ ���� // �ִϸ��̼� Ŭ�� Attack �ȿ� �̺�Ʈ �߰�: ApplyDamage
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

        // ���� �� ����ȭ
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
