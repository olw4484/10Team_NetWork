using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MeleeMinionController : BaseMinionController
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
        Debug.Log("TryAttack() called!");
        if (!PhotonNetwork.IsMasterClient || attackTimer < attackCooldown || attackTarget == null || isDead)
        {
            Debug.Log("TryAttack blocked by condition");
            return;
        }
        var targetPV = attackTarget.GetComponent<PhotonView>();
        if (targetPV == null)
        {
            Debug.LogError("[Minion] TryAttack: attackTarget¿¡ PhotonView ¾øÀ½! " + attackTarget.name);
            return;
        }

        attackTimer = 0f;
        int targetViewID = targetPV.ViewID;
        Debug.Log("TryAttack conditions passed, sending RPC_TryAttack!");
        photonView.RPC(nameof(RPC_TryAttack), RpcTarget.All, targetViewID, attackPower);
    }

    [PunRPC]
    public void RPC_TryAttack(int targetViewID, int dmg)
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

