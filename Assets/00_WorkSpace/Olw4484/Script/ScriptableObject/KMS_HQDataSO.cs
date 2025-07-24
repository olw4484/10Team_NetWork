using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HQDataSO", menuName = "Minion/HQ Data")]
public class KMS_HQDataSO : ScriptableObject
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

    [Header("�ڵ� �̴Ͼ� ���� ��ġ�� (UpLine, DownLine)")]
    public Transform[] autoSpawnPoints;

    [Header("�̴Ͼ� ������")]
    public GameObject minionPrefab;

    [Header("�ڵ� ���� ���� (��)")]
    public float autoSpawnInterval = 5f;

    [Header("�ڵ� ���� �̴Ͼ� �� (����Ʈ��)")]
    public int autoMinionCount = 1;

    [Header("���� ���� ������ �̴Ͼ� ���")]
    public List<SpawnableMinionInfo> manualSpawnList;
}
