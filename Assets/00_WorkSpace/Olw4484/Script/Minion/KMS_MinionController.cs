using UnityEngine;

public class MinionController : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;
    public int maxHP;

    private int currentHP;
    private Transform target;
    private bool isDead = false;
    private float attackTimer = 0f;

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

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void MoveTowards(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    private void TryAttack()
    {
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            // 공격 애니메이션 및 데미지 처리
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

        if (EventManager.Instance != null)
        {
            EventManager.Instance.MinionDead(this, killer);
            EventManager.Instance.MinionKillConfirmed(killer, this);
        }

        Destroy(gameObject, 1f);
    }
}
