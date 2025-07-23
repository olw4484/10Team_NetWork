using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS_ResourceSystem : MonoBehaviour
{
    public enum ResourceType { Gold, Gear }
    public static KMS_ResourceSystem Instance { get; private set; }

    [Header("Resource Settings")]
    public int currentGold = 0;
    public int currentGear = 0; // ���� ��� ���ɼ��� ����.

    // �̱��� �ν��Ͻ�
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // �̴Ͼ� ������ �ʿ��� �ڿ� ����
    public bool HasEnoughResource(MinionType type)
    {
        return currentGold >= GetCost(type);
    }

    // �̴Ͼ� ������ �ڿ� �Ҹ�
    public void ConsumeResource(MinionType type)
    {
        currentGold -= GetCost(type);
    }

    // �̴Ͼ� ������ �Ҹ�� �ڿ��� ��
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
    /// �̴Ͼ� óġ�� ���� �� �ִ� �ڿ��� Ÿ�԰� ��
    /// SO�� �����ؼ� MinionDataSO.goldReward ���� �޾ƿ�
    /// </summary>
    /// <param name="type">ȹ���� �ڿ��� ���� (��: Gold, Gear)</param>
    /// <param name="amount">ȹ���� �ڿ��� ��</param>

    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Gold:
                currentGold += amount;
                Debug.Log($"[Resource] ��� +{amount} �� ����: {currentGold}");
                break;

            case ResourceType.Gear:
                currentGear += amount;
                Debug.Log($"[Resource] ��� +{amount} �� ����: {currentGear}");
                break;

                // �� ���� �ڿ��� ����� ���� �߰�
        }
    }
}

