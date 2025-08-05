using Photon.Pun;
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
        controller.SetManualControl(false); // �ڵ� ��ȯ
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
            Debug.LogError($"[Factory] MinionDataDict���� {type} �����͸� ã�� �� �����ϴ�.");
            return false;
        }

        var controller = go.GetComponent<BaseMinionController>();
        if (controller == null)
        {
            Debug.LogError("[Factory] BaseMinionController�� �����տ� �������� �ʽ��ϴ�.");
            return false;
        }

        controller.Initialize(data, moveTarget: null, attackTarget: null, waypointGroup: null, teamId);

        // ���� ��ȯ �̴Ͼ����� ����
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


