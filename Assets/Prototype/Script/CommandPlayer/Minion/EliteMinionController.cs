using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class EliteMinionController : BaseMinionController
{
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
        if (!PhotonNetwork.IsMasterClient || attackTimer < attackCooldown || attackTarget == null || isDead) return;
            
        var targetPV = attackTarget.GetComponent<PhotonView>();
        if (targetPV != null)
        {
            attackTimer = 0f;
            photonView.RPC(nameof(RPC_Attack), RpcTarget.All, targetPV.ViewID, attackPower);
        }
    }

    private void ApplyEliteEffect(GameObject target)
    {
        // 여기에 엘리트 전용 효과 추가 가능
        // ex: 넉백, 슬로우, 체력 회복, 버프 등
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

            ApplyEliteEffect(targetPV.gameObject);
        }
    }

    [PunRPC]
    public void RPC_SetMoving()
    {
        animator.SetBool("IsMoving", isMoving);
    }


}