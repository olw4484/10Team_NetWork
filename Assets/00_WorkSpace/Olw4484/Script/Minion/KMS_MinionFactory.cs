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

public class KMS_MinionFactory : MonoBehaviour
{
    public static KMS_MinionFactory Instance { get; private set; }

    private Dictionary<KMS_MinionType, KMS_MinionDataSO> minionDataDict = new();
    [SerializeField] private KMS_MinionDataSO[] minionDataList;
    public KMS_HQDataSO hqData;

    [Header("Minion Prefabs")]
    public GameObject meleeMinionPrefab;
    public GameObject rangedMinionPrefab;
    public GameObject eliteMinionPrefab;
    private Dictionary<KMS_MinionType, GameObject> prefabDict;

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
        prefabDict = new Dictionary<KMS_MinionType, GameObject>
        {
            { KMS_MinionType.Melee, meleeMinionPrefab },
            { KMS_MinionType.Ranged, rangedMinionPrefab },
            { KMS_MinionType.Elite, eliteMinionPrefab }
        };

    }

    public KMS_MinionController SpawnFreeMinion(KMS_MinionType type, Vector3 pos, Transform target, KMS_WaypointGroup waypointGroup = null)
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

        var controller = minion.GetComponent<KMS_MinionController>();
        controller?.Initialize(data, target, waypointGroup);
        return controller;
    }


    public bool TrySpawnMinion(KMS_MinionType type, Vector3 position, Transform target, KMS_CommandPlayer player, int teamId)
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

        var controller = go.GetComponent<KMS_MinionController>();
        if (controller == null)
        {
            Debug.LogError("[Factory] MinionController�� �����տ� �������� �ʽ��ϴ�.");
            return false;
        }

        controller.Initialize(data, target, null, teamId);

        return true;
    }

    public GameObject GetMinionPrefab(KMS_MinionType type)
    {
        return type switch
        {
            KMS_MinionType.Melee => meleeMinionPrefab,
            KMS_MinionType.Ranged => rangedMinionPrefab,
            KMS_MinionType.Elite => eliteMinionPrefab,
            _ => null
        };
    }
}
public enum KMS_MinionType { Melee = 0, Ranged = 1, Elite =2 }


