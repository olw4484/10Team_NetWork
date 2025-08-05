using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMinionController : BaseMinionController
{
    protected override void Awake()
    {
        Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] {name} - MeleeMinionController.Awake");
        base.Awake();
    }
    protected override void Start()
    {
        Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] {name} - MeleeMinionController.Start");
        base.Start();
    }
    public override void LocalInitialize(
        MinionDataSO data,
        Transform moveTarget = null,
        Transform attackTarget = null,
        WaypointGroup waypointGroup = null,
        int teamId = 0)
    {
        Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] {name} - MeleeMinionController.Initialize");
        base.LocalInitialize(data, moveTarget, attackTarget, waypointGroup, teamId);
    }

    protected override void TryAttack()
    {
        if (!photonView.IsMine || attackTimer < attackCooldown || attackTarget == null || isDead) return;

        // ���⼭ ���� ����Ʈ or �ִϸ��̼� or ������ ó��
        var targetPV = attackTarget.GetComponent<PhotonView>();
        if (targetPV != null)
        {
            attackTimer = 0f;
            photonView.RPC(nameof(RPC_Attack), RpcTarget.All, targetPV.ViewID, attackPower);
        }
    }

    [PunRPC]
    public void RPC_Attack(int targetViewID, int dmg)
    {
        var targetPV = PhotonView.Find(targetViewID);
        if (targetPV != null)
        {
            var damageable = targetPV.GetComponent<IDamageable>();
            damageable?.TakeDamage(dmg, this.gameObject);
            view?.PlayMinionAttackAnimation();
        }
    }
}

