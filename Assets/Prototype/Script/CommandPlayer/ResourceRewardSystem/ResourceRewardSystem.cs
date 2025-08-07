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
        // 골드 지급
        var data = minion.data;
        if (PhotonNetwork.IsMasterClient)
        {
            var commendPlayer = killer.GetComponent<CommandPlayer>();
            if (commendPlayer != null && data.goldReward > 0)
            {
                commendPlayer.photonView.RPC("RpcAddGold", RpcTarget.All, data.goldReward);
            }

            var heroPlayer = killer.GetComponent<HeroController>();
            if (heroPlayer != null && data.goldReward > 0)
            {
                heroPlayer.pv.RPC("RpcAddGold", RpcTarget.All, data.goldReward);
            }
        }

        // 경험치 지급
        if (PhotonNetwork.IsMasterClient)
        {
            var killerExpHandler = killer.GetComponent<IExpReceiver>();
            if (killerExpHandler != null && data.expReward > 0)
            {
                var photonView = ((MonoBehaviour)killerExpHandler).GetComponent<PhotonView>();
                if (photonView != null)
                    photonView.RPC("RpcAddExp", RpcTarget.All, data.expReward);
            }
        }
    }
}
