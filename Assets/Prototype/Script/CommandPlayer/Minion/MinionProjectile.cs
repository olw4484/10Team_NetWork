using Photon.Pun;
using UnityEngine;

public class MinionProjectile : MonoBehaviour
{
    private int damage;
    private Transform target;
    private GameObject owner;
    private int teamId;
    private float speed = 10f;

    private float maxLifeTime = 5f;
    private float lifeTimer;

    public void Initialize(int damage, Transform target, GameObject owner, int teamId)
    {
        this.damage = damage;
        this.target = target;
        this.owner = owner;
        this.teamId = teamId;
        lifeTimer = 0f;
    }

    private void Update()
    {
        if (target == null)
        {
            DestroyProjectile();
            return;
        }

        // 타겟 방향으로 이동
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // 타겟 도착 판정
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < 0.2f)
        {
            ApplyDamage();
            DestroyProjectile();
        }

        // 최대 수명 초과 시 삭제
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLifeTime)
        {
            DestroyProjectile();
        }
    }

    void ApplyDamage()
    {
        if (target == null) return;

        var pv = target.GetComponent<PhotonView>();
        var damageable = target.GetComponent<IDamageable>();

        if (pv == null || !IsValid(pv) || damageable == null) return;

        // 아군 여부 판별
        var minion = target.GetComponent<BaseMinionController>();
        if (minion != null && minion.teamId == this.teamId)
            return; // 아군 => 무시

        pv.RPC("RPC_TakeDamage", RpcTarget.All, damage, owner.GetComponent<PhotonView>().ViewID);
    }

    private void DestroyProjectile()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.Destroy(gameObject);
        else
            Destroy(gameObject);
    }

    private bool IsValid(PhotonView view)
    {
        return view != null && view.ViewID != 0 && PhotonView.Find(view.ViewID) != null;
    }
}

