using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMinionController : BaseMinionController
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    public override void Initialize(
        MinionDataSO data,
        Transform moveTarget = null,
        Transform attackTarget = null,
        WaypointGroup waypointGroup = null,
        int teamId = 0)
    {
        base.Initialize(data, moveTarget, attackTarget, waypointGroup, teamId);
        // 필요하면 여기서만 추가 세팅
    }

    protected override void TryAttack()
    {
        if (!photonView.IsMine || attackTimer < attackCooldown || attackTarget == null || isDead) return;

        var targetPV = attackTarget.GetComponent<PhotonView>();
        if (targetPV == null)
        {
            Debug.LogError("[Minion] TryAttack: attackTarget에 PhotonView 없음! " + attackTarget.name);
            return;
        }

        attackTimer = 0f;
        int targetViewID = targetPV.ViewID;
        photonView.RPC(nameof(RPC_TryAttack), RpcTarget.All, targetViewID, attackPower);
    }

    [PunRPC]
    private void RPC_TryAttack(int targetViewID, int dmg)
    {
        var targetPV = PhotonView.Find(targetViewID);
        if (targetPV == null || isDead) return;

        view?.PlayMinionAttackAnimation();

        // 탄환 생성
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + transform.forward * 0.5f;
        Quaternion rotation = Quaternion.LookRotation(targetPV.transform.position - spawnPos);

        GameObject proj = Instantiate(projectilePrefab, spawnPos, rotation);

        var projectile = proj.GetComponent<MinionProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(
                damage: dmg,
                target: targetPV.transform,
                owner: gameObject,
                teamId: this.teamId
            );
        }
        // 풀링은 추후 교체
    }
}

