using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KMS_MinionFactory : MonoBehaviour
{
    public static KMS_MinionFactory Instance { get; private set; }

    private Dictionary<MinionType, MinionDataSO> minionDataDict = new();
    [SerializeField] private MinionDataSO[] minionDataList;
    public KMS_HQDataSO hqData;

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

    public MinionController SpawnFreeMinion(MinionType type, Vector3 pos, Transform target, KMS_WaypointGroup waypointGroup = null)
    {
        if (!KMS_MinionFactory.Instance.minionDataDict.TryGetValue(type, out var data))
        {
            Debug.LogError($"[Spawner] 해당 MinionType({type})에 대한 데이터가 없습니다.");
            return null;
        }

        if (data == null)
        {
            Debug.LogError($"[Spawner] 데이터는 있지만 null입니다. Type: {type}");
            return null;
        }

        GameObject minion = Instantiate(prefabDict[type], pos, Quaternion.identity);
        var controller = minion.GetComponent<MinionController>();
        controller?.Initialize(data, target, waypointGroup);
        return controller;
    }


    public bool TrySpawnMinion(MinionType type, Vector3 position, Transform target, KMS_CommandPlayer player)
    {
        if (player == null)
        {
            Debug.LogError("[Factory] Player is null!");
            return false;
        }

        var minionInfo = hqData.manualSpawnList.FirstOrDefault(x => x.type == type);
        if (minionInfo == null)
        {
            Debug.LogError($"[Factory] MinionInfo for type {type} is null!");
            return false;
        }

        if (target == null)
        {
            Debug.LogWarning("[Factory] Target is null - 미니언이 움직이지 않을 수 있음.");
        }

        if (!player.TrySpendGold(minionInfo.cost))
            return false;

        var go = Instantiate(minionInfo.prefab, position, Quaternion.identity);
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

        controller.Initialize(data, target, null);

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
public enum MinionType { Melee = 0, Ranged = 1, Elite =2 }


