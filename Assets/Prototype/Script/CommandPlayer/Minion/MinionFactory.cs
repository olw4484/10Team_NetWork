using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinionFactory : MonoBehaviour
{
    public static MinionFactory Instance { get; private set; }

    private Dictionary<MinionType, MinionDataSO> minionDataDict = new();
    [SerializeField] private MinionDataSO[] minionDataList;
    public HQDataSO hqData;

    [Header("Minion Prefabs")]
    public GameObject meleeMinionPrefab;
    public GameObject rangedMinionPrefab;
    public GameObject eliteMinionPrefab;
    public GameObject m_MeleeMinionPrefab;
    public GameObject m_RangedMinionPrefab;
    public GameObject m_EliteMinionPrefab;
    private Dictionary<MinionType, GameObject> prefabDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var data in minionDataList)
        {
            if (!minionDataDict.ContainsKey(data.minionType))
                minionDataDict.Add(data.minionType, data);
        }
        prefabDict = new Dictionary<MinionType, GameObject>
        {
            { MinionType.Melee, meleeMinionPrefab },
            { MinionType.Ranged, rangedMinionPrefab },
            { MinionType.Elite, eliteMinionPrefab }
        };
    }

    /// <summary>
    /// 자동 미니언을 스폰하는 함수 (웨이포인트 따라 이동)
    /// </summary>
    public void SpawnAutoMinion(MinionType type, Vector3 spawnPos, string groupId, int teamId)
    {
        if (!minionDataDict.TryGetValue(type, out var data) || !prefabDict.TryGetValue(type, out var prefab))
        {
            Debug.LogError($"[Factory] 미니언 데이터 또는 프리팹 없음: {type}");
            return;
        }

        object[] instantiationData = new object[] { (int)type, teamId, groupId , teamId};
        PhotonNetwork.Instantiate(prefab.name, spawnPos, Quaternion.identity, 0, instantiationData);
    }

    /// <summary>
    /// 수동 제어용 미니언을 스폰하는 함수
    /// </summary>
    public bool TrySpawnMinion(MinionType type, Vector3 position, Transform target, CommandPlayer player, int teamId)
    {
        var minionInfo = hqData.manualSpawnList.FirstOrDefault(x => x.type == type);
        if (minionInfo == null)
        {
            Debug.LogError($"[Factory] manualSpawnList에 {type}이 없음");
            return false;
        }
        if (!player.TrySpendGold(minionInfo.cost))
            return false;

        // 수동 미니언 프리팹에서 MinionController가 반드시 붙어있어야 함
        object[] instantiationData = new object[] { (int)type, teamId, "Manual" };
        var go = PhotonNetwork.Instantiate(minionInfo.prefab.name, position, Quaternion.identity, 0, instantiationData);

        // 추가적으로 로컬에서만 필요한 세팅이 있다면
        var ctrl = go.GetComponent<MinionController>();
        ctrl?.SetManualControl(true);
        if (target != null)
            ctrl?.SetMoveTarget(target);

        return true;
    }

    public MinionDataSO GetMinionData(MinionType type)
    {
        if (minionDataDict.TryGetValue(type, out var data))
            return data;
        Debug.LogError($"[Factory] MinionDataSO를 찾을 수 없음: {type}");
        return null;
    }

    public GameObject GetMinionPrefab(MinionType type)
    {
        return type switch
        {
            MinionType.Melee => meleeMinionPrefab,
            MinionType.Ranged => rangedMinionPrefab,
            MinionType.Elite => eliteMinionPrefab,
            _ => null
        };
    }
}
public enum MinionType { Melee = 0, Ranged = 1, Elite = 2, Reinforced_Melee = 3, Reinforced_Ranged = 4 }