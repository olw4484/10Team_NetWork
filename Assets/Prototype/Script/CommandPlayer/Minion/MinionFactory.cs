using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �̴Ͼ� ���� �Լ� (��������/��Ʈ��ũ ��� ���)
/// - ��������: Instantiate()
/// - ��Ʈ��ũ: PhotonNetwork.Instantiate()
///     �� ����: PhotonNetwork.Instantiate()�� �������� �ݵ�� Resources ���� ������ �־�� �ϸ�,
///     ������ �̸��� Resources ���� ��ο� ��ġ�ؾ� �� (��: "Minion_Melee").
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
            Debug.LogError($"[Spawner] MinionType({type}) ������ ����");
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
            Debug.LogWarning("[Factory] Target is null - �̴Ͼ��� �������� ���� �� ����.");
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
            Debug.LogError("[Factory] Instantiate ����� null�Դϴ�.");
            return false;
        }

        if (!minionDataDict.TryGetValue(type, out var data))
        {
            Debug.LogError($"[Factory] MinionDataDict���� {type} �����͸� ã�� �� �����ϴ�.");
            return false;
        }

        var controller = go.GetComponent<MinionController>();
        if (controller == null)
        {
            Debug.LogError("[Factory] MinionController�� �����տ� �������� �ʽ��ϴ�.");
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


