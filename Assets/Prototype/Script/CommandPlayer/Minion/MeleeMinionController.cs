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
        if (!photonView.IsMine || isDead || target == null) return;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            // 애니메이션 실행
            view?.PlayMinionAttackAnimation();

            // 대상에게 데미지 적용
            var targetDamageable = target.GetComponent<IDamageable>();
            if (targetDamageable != null)
                targetDamageable.TakeDamage(attackPower, gameObject);
        }
    }
}
