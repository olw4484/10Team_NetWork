using UnityEngine;

[CreateAssetMenu(fileName = "HQDataSO", menuName = "ScriptableObject/HQ Data")]
public class KMS_HQDataSO : ScriptableObject
{
    [Header("본진 HQ 프리팹 (HQ_Root 기준)")]
    public GameObject hqPrefab;

    [Header("최대 체력")]
    public int maxHP = 1000;

    [Header("자동 미니언 생성 위치들 (UpLine, DownLine)")]
    public Transform[] autoSpawnPoints;

    [Header("수동 미니언 대기 위치")]
    public Transform manualSpawnPoint;

    [Header("미니언 프리팹")]
    public GameObject minionPrefab;

    [Header("자동 생성 간격 (초)")]
    public float autoSpawnInterval = 5f;

    [Header("자동 생성 미니언 수 (포인트당)")]
    public int autoMinionCount = 1;
}
