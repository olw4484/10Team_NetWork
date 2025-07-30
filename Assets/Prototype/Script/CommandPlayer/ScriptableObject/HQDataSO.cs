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

    [Header("���� HQ ������ (HQ_Root ����)")]
    public GameObject hqPrefab;

    [Header("�ִ� ü��")]
    public int maxHP = 1000;

    [Header("���� ���� ������ �̴Ͼ� ���")]
    public List<SpawnableMinionInfo> manualSpawnList;
}
