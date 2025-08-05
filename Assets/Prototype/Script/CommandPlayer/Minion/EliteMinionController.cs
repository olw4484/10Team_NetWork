using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class EliteMinionController : BaseMinionController
{
    public override void Initialize(
        MinionDataSO data,
        Transform moveTarget = null,
        Transform attackTarget = null,
        WaypointGroup waypointGroup = null,
        int teamId = 0)
    {
        base.Initialize(data, moveTarget, attackTarget, waypointGroup, teamId);
    }

    protected override void TryAttack()
    {
        if (!photonView.IsMine || attackTimer < attackCooldown || attackTarget == null || isDead) return;

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

    private void ApplyEliteEffect(GameObject target)
    {
        // 여기에 엘리트 전용 효과 추가 가능
        // ex: 넉백, 슬로우, 체력 회복, 버프 등
    }
}
