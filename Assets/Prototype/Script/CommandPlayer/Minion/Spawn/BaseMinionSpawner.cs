using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMinionSpawner : MonoBehaviour
{
    protected void SpawnFreeMinion(MinionType type, Vector3 pos, Transform target, WaypointGroup waypointGroup = null)
    {
        MinionFactory.Instance.SpawnFreeMinion(type, pos, target, waypointGroup);
    }
}
