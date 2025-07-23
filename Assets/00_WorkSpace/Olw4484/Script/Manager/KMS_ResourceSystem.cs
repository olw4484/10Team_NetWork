using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS_ResourceSystem : MonoBehaviour
{
    public enum ResourceType { Gold, Gear }
    public static KMS_ResourceSystem Instance { get; private set; }

    [Header("Resource Settings")]
    public int currentGold = 0;
    public int currentGear = 0; // 추후 사용 가능성이 있음.

    // 싱글턴 인스턴스
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 미니언 생성시 필요한 자원 조건
    public bool HasEnoughResource(MinionType type)
    {
        return currentGold >= GetCost(type);
    }

    // 미니언 생성시 자원 소모
    public void ConsumeResource(MinionType type)
    {
        currentGold -= GetCost(type);
    }

    // 미니언 생성시 소모될 자원의 값
    public int GetCost(MinionType type)
    {
        return type switch
        {
            MinionType.Melee => 50,
            MinionType.Ranged => 75,
            MinionType.Elite => 150,
            _ => 0
        };
    }
    /// <summary>
    /// 미니언 처치시 얻을 수 있는 자원의 타입과 양
    /// SO를 추적해서 MinionDataSO.goldReward 값을 받아옴
    /// </summary>
    /// <param name="type">획득할 자원의 종류 (예: Gold, Gear)</param>
    /// <param name="amount">획득할 자원의 양</param>

    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Gold:
                currentGold += amount;
                Debug.Log($"[Resource] 골드 +{amount} → 현재: {currentGold}");
                break;

            case ResourceType.Gear:
                currentGear += amount;
                Debug.Log($"[Resource] 기어 +{amount} → 현재: {currentGear}");
                break;

                // 더 많은 자원이 생기면 여기 추가
        }
    }
}

