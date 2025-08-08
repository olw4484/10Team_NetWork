using Photon.Pun;
using UnityEngine;

public class GoldTickSystem : MonoBehaviour
{
    private CommandPlayer player; // CommandPlayer 변수 선언
    public int goldPerTick = 10; // 틱당 얻는 골드량
    public float tickInterval = 5f; // 틱 간격 (초)
    private float nextTickTime; // 다음 틱이 발생할 시간

    void Awake()
    {
        // 씬에서 CommandPlayer 컴포넌트를 가진 오브젝트를 찾아서 할당
        // 이 오브젝트가 씬에 없으면 player 변수는 여전히 null이 됩니다.
        player = FindObjectOfType<CommandPlayer>();
        nextTickTime = Time.time + tickInterval;
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