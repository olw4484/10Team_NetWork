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

    public MinionController SpawnFreeMinion(MinionType type, Vector3 pos, Transform target, WaypointGroup waypointGroup = null, int teamId = 0)
    {
        if (!minionDataDict.TryGetValue(type, out var data) || data == null)
        {
            Debug.LogError($"[Spawner] MinionType({type}) 데이터 오류");
            return null;
        }

        GameObject minion;
        if (Photon.Pun.PhotonNetwork.InRoom)
        {
            minion = Photon.Pun.PhotonNetwork.Instantiate(prefabDict[type].name, pos, Quaternion.identity);
        }
        else
        {
            minion = Instantiate(prefabDict[type], pos, Quaternion.identity);
        }

        var controller = minion.GetComponent<MinionController>();
        controller?.Initialize(data, target, waypointGroup, teamId);
        return controller;
    }


    public bool TrySpawnMinion(MinionType type, Vector3 position, Transform target, CommandPlayer player, int teamId)
    {

        var minionInfo = hqData.manualSpawnList.FirstOrDefault(x => x.type == type);

        if (target == null)
        {
            Debug.LogWarning("[Factory] Target is null - 미니언이 움직이지 않을 수 있음.");
        }

        if (!player.TrySpendGold(minionInfo.cost))
            return false;

        GameObject go;

        if (Photon.Pun.PhotonNetwork.InRoom)
        {
            var prefabName = minionInfo.prefab.name;
            go = Photon.Pun.PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);
        }
        else
        {
            go = Instantiate(minionInfo.prefab, position, Quaternion.identity);
        }

        if (go == null)
        {
            Debug.LogError("[Factory] Instantiate 결과가 null입니다.");
            return false;
        }

        if (!minionDataDict.TryGetValue(type, out var data))
        {
            Debug.LogError($"[Factory] MinionDataDict에서 {type} 데이터를 찾을 수 없습니다.");
            return false;
        }

        var controller = go.GetComponent<MinionController>();
        if (controller == null)
        {
            Debug.LogError("[Factory] MinionController가 프리팹에 존재하지 않습니다.");
            return false;
        }

        controller.Initialize(data, target, null, teamId);

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


