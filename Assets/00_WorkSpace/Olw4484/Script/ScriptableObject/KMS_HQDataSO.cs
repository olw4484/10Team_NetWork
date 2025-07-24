using UnityEngine;

[CreateAssetMenu(fileName = "HQDataSO", menuName = "ScriptableObject/HQ Data")]
public class KMS_HQDataSO : ScriptableObject
{
    [Header("���� HQ ������ (HQ_Root ����)")]
    public GameObject hqPrefab;

    [Header("�ִ� ü��")]
    public int maxHP = 1000;

    [Header("�ڵ� �̴Ͼ� ���� ��ġ�� (UpLine, DownLine)")]
    public Transform[] autoSpawnPoints;

    [Header("���� �̴Ͼ� ��� ��ġ")]
    public Transform manualSpawnPoint;

    [Header("�̴Ͼ� ������")]
    public GameObject minionPrefab;

    [Header("�ڵ� ���� ���� (��)")]
    public float autoSpawnInterval = 5f;

    [Header("�ڵ� ���� �̴Ͼ� �� (����Ʈ��)")]
    public int autoMinionCount = 1;
}
