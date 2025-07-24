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

    // 공격 시도
    private void TryAttack()
    {
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            if (view != null && view.Animator != null)
                ; //view.Animator.SetFloat("AttackSpeed", data.AttackAnimSpeed); //공격속도가 필요하면 주석 해제하고 사용 가능

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
