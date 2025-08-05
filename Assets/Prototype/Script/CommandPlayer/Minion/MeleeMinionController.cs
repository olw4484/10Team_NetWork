using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMinionController : BaseMinionController
{
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

        // 여기서 어택 이펙트 or 애니메이션 or 데미지 처리
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

