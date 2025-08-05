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
            {
                minionDataDict.Add(data.minionType, data);
            }
            else
            {
                Debug.LogWarning($"Duplicate MinionType: {data.minionType}");
            }
        }
        prefabDict = new Dictionary<MinionType, GameObject>
        {
            { MinionType.Melee, meleeMinionPrefab },
            { MinionType.Ranged, rangedMinionPrefab },
            { MinionType.Elite, eliteMinionPrefab }
        };

    }

    public BaseMinionController SpawnAutoMinion(MinionType type, Vector3 spawnPos, WaypointGroup waypointGroup, int teamId)
    {
        if (!minionDataDict.TryGetValue(type, out var data))
        {
            Debug.LogError($"[Factory] MinionData for {type} not found.");
            return null;
        }

        if (!prefabDict.TryGetValue(type, out var prefab))
        {
            Debug.LogError($"[Factory] Prefab for {type} not found.");
            return null;
        }

        GameObject go;
        if (PhotonNetwork.InRoom)
            go = PhotonNetwork.Instantiate(prefab.name, spawnPos, Quaternion.identity);
        else
            go = Instantiate(prefab, spawnPos, Quaternion.identity);

        var controller = go.GetComponent<BaseMinionController>();
        if (controller == null)
        {
            Debug.LogError("[Factory] No BaseMinionController on prefab.");
            return null;
        }

        controller.Initialize(data, moveTarget: null, attackTarget: null, waypointGroup, teamId);
        controller.SetManualControl(false); // 자동 소환
        controller.SetWaypointGroup(waypointGroup);
        return controller;
    }


    public bool TrySpawnMinion(MinionType type, Vector3 position, Transform target, CommandPlayer player, int teamId)
    {
        var minionInfo = hqData.manualSpawnList.FirstOrDefault(x => x.type == type);
        if (!player.TrySpendGold(minionInfo.cost)) return false;

        GameObject go;
        if (Photon.Pun.PhotonNetwork.InRoom)
            go = Photon.Pun.PhotonNetwork.Instantiate(minionInfo.prefab.name, position, Quaternion.identity);
        else
            go = Instantiate(minionInfo.prefab, position, Quaternion.identity);

        if (!minionDataDict.TryGetValue(type, out var data))
        {
            Debug.LogError($"[Factory] MinionDataDict에서 {type} 데이터를 찾을 수 없습니다.");
            return false;
        }

        var controller = go.GetComponent<BaseMinionController>();
        if (controller == null)
        {
            Debug.LogError("[Factory] BaseMinionController가 프리팹에 존재하지 않습니다.");
            return false;
        }

        controller.Initialize(data, moveTarget: null, attackTarget: null, waypointGroup: null, teamId);

        // 수동 소환 미니언으로 설정
        controller.SetManualControl(true);

        return true;
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


