using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS_BaseMinionSpawner : MonoBehaviour
{
    public MinionController SpawnFreeMinion(MinionType type, Vector3 position, Transform target)
    {
        GameObject prefab = KMS_MinionFactory.Instance.GetMinionPrefab(type);
        if (prefab == null) return null;

        GameObject minion = Instantiate(prefab, position, Quaternion.identity);
        MinionController controller = minion.GetComponent<MinionController>();
        controller?.SetTarget(target);

        return controller;
    }
}
