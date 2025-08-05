using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMinionController : BaseMinionController
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab; // 추후 풀링으로 교체 가능
    [SerializeField] private Transform firePoint;

    public override void Initialize(MinionDataSO data, Transform target, WaypointGroup waypointGroup = null, int teamId = 0)
    {
        base.Initialize(data, target, waypointGroup, teamId);

        IsManual = false;
        isFollowingWaypoint = false;
    }
    protected override void TryAttack()
    {
        if (!photonView.IsMine || attackTimer < attackCooldown || target == null || isDead) return;

        var targetPV = target.GetComponent<PhotonView>();
        if (targetPV == null)
        {
            Debug.LogError("[Minion] TryAttack: target에 PhotonView 없음! " + target.name);
            return;
        }

        attackTimer = 0f;
        int targetViewID = targetPV.ViewID;
        photonView.RPC(nameof(RPC_TryAttack), RpcTarget.All, targetViewID);
    }

    [PunRPC]
    private void RPC_TryAttack(int targetViewID)
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
                damage: attackPower,
                target: targetPV.transform,
                owner: gameObject,
                teamId: this.teamId
            );
        }

        // TODO: 풀링 사용할 경우 Instantiate → PoolManager.Spawn 으로 교체
    }
}

