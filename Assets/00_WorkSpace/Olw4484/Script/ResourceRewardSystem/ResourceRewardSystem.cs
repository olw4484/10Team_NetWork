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

        if (KMS_ResourceSystem.Instance != null)
        {
            if (data.goldReward > 0)
                KMS_ResourceSystem.Instance.AddResource(KMS_ResourceSystem.ResourceType.Gold, data.goldReward);

            if (data.gearReward > 0)
                KMS_ResourceSystem.Instance.AddResource(KMS_ResourceSystem.ResourceType.Gear, data.gearReward);
        }
    }
}
