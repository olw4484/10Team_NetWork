using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS_ResourceSystem : MonoBehaviour
{
    public static KMS_ResourceSystem Instance { get; private set; }

    [Header("Resource Settings")]
    public int currentGold = 500;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool HasEnoughResource(MinionType type)
    {
        return currentGold >= GetCost(type);
    }

    public void ConsumeResource(MinionType type)
    {
        currentGold -= GetCost(type);
    }

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
}

