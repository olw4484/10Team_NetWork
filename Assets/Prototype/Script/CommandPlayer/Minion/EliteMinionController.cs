using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EliteMinionController : BaseMinionController
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

        attackTimer = 0f;

        int targetViewID = target.GetComponent<PhotonView>().ViewID;
        photonView.RPC(nameof(RPC_TryAttack), RpcTarget.All, targetViewID);
    }

    [PunRPC]
    private void RPC_TryAttack(int targetViewID)
    {
        var targetPV = PhotonView.Find(targetViewID);
        if (targetPV == null || isDead) return;

        view?.PlayMinionAttackAnimation();

        var damageable = targetPV.GetComponent<IDamageable>();
        if (damageable == null)
        {
            Debug.LogWarning($"{targetPV.name} is not IDamageable");
            return;
        }

        damageable.TakeDamage(attackPower, gameObject);
        ApplyEliteEffect(targetPV.gameObject);
    }

    private void ApplyEliteEffect(GameObject target)
    {
        // 여기에 엘리트 전용 효과 추가 가능
        // ex: 넉백, 슬로우, 체력 회복, 버프 등
    }
}
