using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS_BaseMinionSpawner : MonoBehaviour
{
    protected MinionController SpawnMinion(MinionType type, Vector3 pos, Transform target)
    {
        return KMS_MinionFactory.Instance.CreateMinion(type, pos, target);
    }
}
