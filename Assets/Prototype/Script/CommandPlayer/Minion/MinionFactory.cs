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
            Debug.LogError($"[Factory] �̴Ͼ� ������ �Ǵ� ������ ����: {type}");
            return;
        }

        GameObject go;
        PhotonView pv;
        if (PhotonNetwork.InRoom)
        {
            go = PhotonNetwork.Instantiate(prefab.name, spawnPos, Quaternion.identity);
            pv = go.GetComponent<PhotonView>();

            // ---- �߿�: ���� �� ��ٷ� RPC�� �ʱ�ȭ ������ ���� ----
            int waypointID = waypointGroup != null ? waypointGroup.GetInstanceID() : -1; // ��������Ʈ ��Ʈ��ũ�� �ĺ��� �ʿ��

            pv.RPC(nameof(BaseMinionController.RpcInitialize),
                RpcTarget.All,
                (int)type, teamId, waypointID /* �ʿ��� �ٸ� ������ �߰� */);
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
                (int)type, teamId, -1 /* ������ ��������Ʈ ����, �ʿ��� ���� �߰� */);
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
        Debug.LogError($"[Factory] MinionDataSO�� ã�� �� ����: {type}");
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


