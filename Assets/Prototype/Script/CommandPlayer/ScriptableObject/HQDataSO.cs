using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HQDataSO", menuName = "Minion/HQ Data")]
public class HQDataSO : ScriptableObject
{
    [System.Serializable]
    public class SpawnableMinionInfo
    {
        public MinionType type;
        public GameObject prefab;
        public int cost;
    }

    [Header("본진 HQ 프리팹 (HQ_Root 기준)")]
    public GameObject hqPrefab;

    [Header("최대 체력")]
    public int maxHP = 1000;

    [Header("수동 생성 가능한 미니언 목록")]
    public List<SpawnableMinionInfo> manualSpawnList;
}
