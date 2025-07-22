using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS_MinionFactory : MonoBehaviour
{
    public static KMS_MinionFactory Instance { get; private set; }

    [Header("Minion Prefabs")]
    public GameObject meleeMinionPrefab;
    public GameObject rangedMinionPrefab;
    public GameObject eliteMinionPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public MinionController CreateMinion(MinionType type, Vector3 spawnPosition, Transform target)
    {
        GameObject prefab = GetPrefabByType(type);
        if (prefab == null)
        {
            Debug.LogWarning($"No prefab found for {type}");
            return null;
        }

        GameObject newMinion = Instantiate(prefab, spawnPosition, Quaternion.identity);
        MinionController controller = newMinion.GetComponent<MinionController>();
        controller.SetTarget(target);
        return controller;
    }

    private GameObject GetPrefabByType(MinionType type)
    {
        return type switch
        {
            MinionType.Melee => meleeMinionPrefab,
            MinionType.Ranged => rangedMinionPrefab,
            MinionType.Elite => eliteMinionPrefab,
            _ => null
        };
    }
}
public enum MinionType { Melee, Ranged, Elite }


