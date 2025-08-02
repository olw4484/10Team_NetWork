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

            // �ִϸ��̼� ����
            view?.PlayMinionAttackAnimation();

            // ��󿡰� ������ ����
            var targetDamageable = target.GetComponent<IDamageable>();
            if (targetDamageable != null)
                targetDamageable.TakeDamage(attackPower, gameObject);
        }
    }
}
