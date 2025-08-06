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
        base.Start();

        if (photonView.IsMine)
        {
            Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] {name} - MeleeMinionController.Start - IsMine: TRUE");
        }
        else
        {
            Debug.Log($"[{PhotonNetwork.LocalPlayer.ActorNumber}] {name} - MeleeMinionController.Start - IsMine: FALSE");
        }
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

