using UnityEngine;

public class GoldTickSystem : MonoBehaviour
{
    [SerializeField] private float tickInterval = 1f;
    [SerializeField] private int goldPerTick = 1;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= tickInterval)
        {
            timer -= tickInterval;
            /// <summary>
            /// PlayerManager 생성 후 주석 해제시 사용가능
            /// PlayerManager.Instance.AllPlayers 함수 필요
            /// <summary>

            //foreach (var player in PlayerManager.Instance.AllPlayers)
            //{
            //    KMS_ResourceSystem.Instance.AddResourceToPlayer(player, ResourceType.Gold, goldPerTick);
            //}
        }
    }
}
