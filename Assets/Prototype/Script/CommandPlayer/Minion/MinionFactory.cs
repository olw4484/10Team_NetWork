using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 미니언 스폰 함수 (오프라인/네트워크 모두 사용)
/// - 오프라인: Instantiate()
/// - 네트워크: PhotonNetwork.Instantiate()
///     ※ 주의: PhotonNetwork.Instantiate()는 프리팹이 반드시 Resources 폴더 하위에 있어야 하며,
///     프리팹 이름이 Resources 폴더 경로와 일치해야 함 (예: "Minion_Melee").
/// </summary>

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

    public void SpawnAutoMinion(MinionType type, Vector3 spawnPos, WaypointGroup waypointGroup, int teamId)
    {
        if (!minionDataDict.TryGetValue(type, out var data) || !prefabDict.TryGetValue(type, out var prefab))
        {
            Debug.LogError($"[Factory] 미니언 데이터 또는 프리팹 없음: {type}");
            return;
        }

        GameObject go;
        PhotonView pv;
        if (PhotonNetwork.InRoom)
        {
            go = PhotonNetwork.Instantiate(prefab.name, spawnPos, Quaternion.identity);
            pv = go.GetComponent<PhotonView>();

            // ---- 중요: 생성 후 곧바로 RPC로 초기화 정보를 보냄 ----
            int waypointID = waypointGroup != null ? waypointGroup.GetInstanceID() : -1; // 웨이포인트 네트워크상 식별자 필요시

            pv.RPC(nameof(BaseMinionController.RpcInitialize),
                RpcTarget.All,
                (int)type, teamId, waypointID /* 필요한 다른 정보도 추가 */);
        }
        else
        {
            go = Instantiate(prefab, spawnPos, Quaternion.identity);
            var ctrl = go.GetComponent<BaseMinionController>();
            ctrl?.LocalInitialize(data, moveTarget: null, attackTarget: null, waypointGroup, teamId);
        }
    }


    public bool TrySpawnMinion(MinionType type, Vector3 position, Transform target, CommandPlayer player, int teamId)
    {
        var minionInfo = hqData.manualSpawnList.FirstOrDefault(x => x.type == type);
        if (!player.TrySpendGold(minionInfo.cost)) return false;

        GameObject go;
        PhotonView pv;
        if (PhotonNetwork.InRoom)
        {
            go = PhotonNetwork.Instantiate(minionInfo.prefab.name, position, Quaternion.identity);
            pv = go.GetComponent<PhotonView>();
            pv.RPC(nameof(BaseMinionController.RpcInitialize),
                RpcTarget.All,
                (int)type, teamId, -1 /* 수동은 웨이포인트 없음, 필요한 정보 추가 */);
        }
        else
        {
            go = Instantiate(minionInfo.prefab, position, Quaternion.identity);
            var ctrl = go.GetComponent<BaseMinionController>();
            ctrl?.LocalInitialize(minionDataDict[type], moveTarget: null, attackTarget: target, waypointGroup: null, teamId);
        }

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
public enum MinionType { Melee = 0, Ranged = 1, Elite =2 , Reinforced_Melee = 3, Reinforced_Ranged = 4 }


