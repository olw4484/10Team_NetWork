using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMinionSpawner : MonoBehaviour
{
    protected void SpawnAutoMinion(MinionType type, Vector3 spawnPos, WaypointGroup waypointGroup, int teamId)
    {
        MinionFactory.Instance.SpawnAutoMinion(type, spawnPos, waypointGroup, teamId);
    }
}
