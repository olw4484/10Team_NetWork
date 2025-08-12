using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceSystem;

public class MinionRewardSystem : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Instance.OnMinionDead += HandleMinionDead;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.OnMinionDead -= HandleMinionDead;
    }

    private void HandleMinionDead(MinionController minion, GameObject killer)
    {
        var data = minion.data;
        if (!PhotonNetwork.IsMasterClient) return;

        int reward = data.goldReward;
        if (reward <= 0) return;

        var minionKiller = killer.GetComponent<BaseMinionController>();
        if (minionKiller != null)
        {
            var ownerCmd = PlayerManager.Instance.GetCommandPlayerByTeam(minionKiller.teamId);
            if (ownerCmd != null)
                ownerCmd.photonView.RPC("RpcAddGold", RpcTarget.All, reward);
            return;
        }

        var commandPlayer = killer.GetComponent<CommandPlayer>();
        if (commandPlayer != null)
        {
            commandPlayer.photonView.RPC("RpcAddGold", RpcTarget.All, reward);
            return;
        }

        var heroPlayer = killer.GetComponent<HeroController>();
        if (heroPlayer != null)
        {
            heroPlayer.pv.RPC("RpcAddGold", RpcTarget.All, reward);
            return;
        }
    }
}
