using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedMinionController : BaseMinionController
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
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

        if (photonView.IsMine)
        {
            object[] instantiationData = new object[]
            {
                dmg,
                targetViewID,
                photonView.ViewID,
                teamId
            };

            PhotonNetwork.Instantiate(
                projectilePrefab.name,
                spawnPos,
                rotation,
                0,
                instantiationData
            );
        }
    }
}

