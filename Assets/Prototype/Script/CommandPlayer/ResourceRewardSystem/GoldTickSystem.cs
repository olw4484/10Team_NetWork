using Photon.Pun;
using UnityEngine;

public class GoldTickSystem : MonoBehaviour
{
    private CommandPlayer player; // CommandPlayer 변수 선언
    public int goldPerTick = 10; // 틱당 얻는 골드량
    public float tickInterval = 5f; // 틱 간격 (초)
    private float nextTickTime; // 다음 틱이 발생할 시간

    private void Awake()
    {
        // PlayerManager의 이벤트에 구독
        PlayerManager.OnPlayerRegistered += OnPlayerRegistered;
    }

    private void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 구독 해제 (메모리 누수 방지)
        PlayerManager.OnPlayerRegistered -= OnPlayerRegistered;
    }

    private void OnPlayerRegistered(CommandPlayer registeredPlayer)
    {
        // PlayerManager로부터 받은 유효한 플레이어 참조를 할당
        player = registeredPlayer;

        Debug.Log("플레이어가 등록되어 GoldTickSystem이 작동을 시작합니다.");
    }

    void Update()
    {
        // player가 null이 아닐 때만 로직을 실행
        if (player != null)
        {
            if (Time.time >= nextTickTime)
            {
                // ResourceSystem의 인스턴스가 존재하는지 확인
                if (ResourceSystem.Instance != null)
                {
                    ResourceSystem.Instance.AddResourceToPlayer(player, ResourceSystem.ResourceType.Gold, goldPerTick);
                    nextTickTime = Time.time + tickInterval;
                }
                else
                {
                    Debug.LogError("ResourceSystem.Instance가 존재하지 않습니다.");
                }
            }
        }
        else
        {
            // player가 null일 경우 로그를 출력하여 디버깅에 도움
            Debug.LogError("CommandPlayer가 씬에 존재하지 않습니다. GoldTickSystem이 올바르게 동작할 수 없습니다.");
        }
    }
}