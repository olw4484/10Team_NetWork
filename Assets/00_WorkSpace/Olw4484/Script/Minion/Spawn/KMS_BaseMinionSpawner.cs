using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS_BaseMinionSpawner : MonoBehaviour
{
    protected void SpawnFreeMinion(KMS_MinionType type, Vector3 pos, Transform target, KMS_WaypointGroup waypointGroup = null)
    {
        KMS_MinionFactory.Instance.SpawnFreeMinion(type, pos, target, waypointGroup);
    }
}
