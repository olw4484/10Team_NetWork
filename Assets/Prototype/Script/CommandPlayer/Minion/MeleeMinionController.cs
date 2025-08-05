using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMinionController : BaseMinionController
{
    public override void Initialize(MinionDataSO data, Transform target, WaypointGroup waypointGroup = null, int teamId = 0)
    {
        base.Initialize(data, target, waypointGroup, teamId);

        IsManual = false;
        isFollowingWaypoint = false;
    }

    protected override void TryAttack()
    {
        if (!photonView.IsMine || attackTimer < attackCooldown || target == null || isDead) return;

        Debug.Log($"[Minion] TryAttack 대상: {target?.name} / Tag: {target?.tag}");

        var targetPV = target.GetComponent<PhotonView>();
        if (targetPV == null)
        {
            Debug.LogError($"[Minion] TryAttack: target에 PhotonView 없음! target: {target?.name}");
            return;
        }

        attackTimer = 0f;
        int targetViewID = targetPV.ViewID;
        photonView.RPC(nameof(RPC_TryAttack), RpcTarget.All, targetViewID);
    }

    [PunRPC]
    private void RPC_TryAttack(int targetViewID)
    {
        view?.PlayMinionAttackAnimation();
        // 추상화된 처리거나, Ranged/Melee에서 오버라이드할 부분
    }
}
