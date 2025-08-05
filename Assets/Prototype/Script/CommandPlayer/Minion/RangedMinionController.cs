using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMinionController : BaseMinionController
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab; // ���� Ǯ������ ��ü ����
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
            Debug.LogError("[Minion] TryAttack: target�� PhotonView ����! " + target.name);
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

        // źȯ ����
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

        // TODO: Ǯ�� ����� ��� Instantiate �� PoolManager.Spawn ���� ��ü
    }
}

