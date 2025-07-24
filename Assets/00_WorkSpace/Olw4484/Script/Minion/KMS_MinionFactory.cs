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
        var minionInfo = hqData.manualSpawnList.FirstOrDefault(x => x.type == type);
        if (minionInfo == null)
        {
            Debug.LogWarning($"[Factory] {type} 미니언 정보를 찾을 수 없음");
            return false;
        }

        if (!player.TrySpendGold(minionInfo.cost))
            return false;

        var go = Instantiate(minionInfo.prefab, position, Quaternion.identity);
        if (minionDataDict.TryGetValue(type, out var data))
        {
            go.GetComponent<MinionController>()?.Initialize(data, target);
        }

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
public enum MinionType { Melee, Ranged, Elite }


