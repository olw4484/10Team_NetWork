using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMinionSpawner : MonoBehaviour
{
    protected void SpawnAutoMinion(MinionType type, Vector3 spawnPos, string groupId, int teamId)
    {
        MinionFactory.Instance.SpawnAutoMinion(type, spawnPos, groupId, teamId);
    }
}
