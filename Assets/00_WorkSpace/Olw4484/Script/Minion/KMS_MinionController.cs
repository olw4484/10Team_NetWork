using System.Resources;
using UnityEngine;
using static KMS_ResourceSystem;

public class MinionController : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;
    public int attackPower;
    public int maxHP;
    public MinionView view;

    public MinionDataSO data;

    private int currentHP;
    private Transform target;
    private bool isDead = false;
    private float attackTimer = 0f;

    private void Awake()
    {
        view = GetComponentInChildren<MinionView>();
    }


    void Start()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        if (isDead) return;

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

    public void Initialize(MinionDataSO data, Transform target)
    {
        this.moveSpeed = data.moveSpeed;
        this.attackRange = data.attackRange;
        this.attackCooldown = data.attackCooldown;
        this.attackPower = data.attackPower;
        this.maxHP = data.maxHP;
        this.currentHP = maxHP;

        this.target = target;
    }


    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void MoveTowards(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    // ���� �õ�
    private void TryAttack()
    {
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            if (view != null && view.Animator != null)
                ; //view.Animator.SetFloat("AttackSpeed", data.AttackAnimSpeed); //���ݼӵ��� �ʿ��ϸ� �ּ� �����ϰ� ��� ����

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

        Destroy(gameObject, 1f);
    }
}
