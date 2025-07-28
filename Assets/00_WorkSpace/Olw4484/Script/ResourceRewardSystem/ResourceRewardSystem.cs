using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KMS_ResourceSystem;

public class KMS_MinionRewardSystem : MonoBehaviour
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

        var commendPlayer = killer.GetComponent<KMS_CommandPlayer>();
        if (commendPlayer != null && data.goldReward > 0)
        {
            commendPlayer.AddGold(data.goldReward);
        }

        // 경험치 지급 //경험치 시스템 필요시 주석 해제후 사용 가능
        //var killerExpHandler = killer.GetComponent<IExpReceiver>();
        //if (killerExpHandler != null && data.expReward > 0)
        //{
        //    killerExpHandler.AddExp(data.expReward);
        //}
    }
}
