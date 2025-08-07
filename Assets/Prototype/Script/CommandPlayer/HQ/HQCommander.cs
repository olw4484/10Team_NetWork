using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using static ISelectable;
using static UnityEngine.GraphicsBuffer;

public class HQCommander : MonoBehaviour, ISelectable, IPunInstantiateMagicCallback
{
    [Header("Linked")]
    public CommandPlayer player;

    [Header("TeamId")]
    public int teamId;

    [Header("SpawnPoint/Target")]
    public Transform defaultSpawnPoint;
    public Transform rallyPointTarget;
    public GameObject rallyPointMarker;
    public WaypointGroup waypointGroup;

    bool IsSelected = false;

    private void Update()
    {
        if (!IsSelected) return;

        if (Input.GetKeyDown(KeyCode.Q))
            OnSpawnMinionButton((int)MinionType.Melee);

        if (Input.GetKeyDown(KeyCode.W))
            OnSpawnMinionButton((int)MinionType.Ranged);

        if (Input.GetKeyDown(KeyCode.E))
            OnSpawnMinionButton((int)MinionType.Elite);

        // Ư�� ��ų Ȯ�� ���ɼ� ) HQSkil 2~3��
    }

    public void OnSpawnMinionButton(int type)
    {
        var minionType = (MinionType)type;
        var spawnPos = defaultSpawnPoint.position;

        // NavMesh ���� ��ġ ����
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 3f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
            Debug.Log($"[HQCommander] NavMesh ���� ����: ��ġ={spawnPos}");
        }
        else
        {
            Debug.LogError("[HQCommander] NavMesh ������ �̴Ͼ��� ������ �� �����ϴ�! ���� ����Ʈ ��ġ ��Ȯ�� �ʿ�");
            return;
        }

        var target = rallyPointTarget != null ? rallyPointTarget : null;

        if (player == null)
        {
            Debug.LogError("[HQCommander] player�� null��");
            return;
        }
        if (MinionFactory.Instance == null)
        {
            Debug.LogError("[HQCommander] MinionFactory.Instance�� null��");
            return;
        }
        if (MinionFactory.Instance.hqData == null)
        {
            Debug.LogError("[HQCommander] MinionFactory.Instance.hqData�� null��");
            return;
        }
        if (MinionFactory.Instance.hqData.manualSpawnList == null)
        {
            Debug.LogError("[HQCommander] manualSpawnList�� null��");
            return;
        }

        int teamId = player.teamId;

        Debug.Log($"[HQCommander] TrySpawnMinion ȣ��: minionType={minionType}, teamId={teamId}");
        bool result = MinionFactory.Instance.TrySpawnMinion(minionType, spawnPos, target, player, teamId);

        if (!result)
            Debug.LogError("[HQCommander] TrySpawnMinion ����");
        else
            Debug.Log("[HQCommander] TrySpawnMinion ����");
    }

    public void SetRallyPoint(Vector3 point)
    {
        rallyPointTarget.position = point;

        if (rallyPointMarker != null)
        {
            rallyPointMarker.transform.position = point;
            rallyPointMarker.SetActive(true);
        }

        Debug.Log($"[Commander] Rally Point ����: {point}");
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            teamId = (int)data[0];
            Debug.Log($"[HQCommander] teamId ����ȭ: {teamId}");
        }
    }

    public void Select()
    {
        IsSelected = true;
        // UI ǥ�� ��� ���
    }

    public void Deselect()
    {
        IsSelected = false;
        // UI ǥ�� ��� ����
    }

    public SelectableType GetSelectableType() => SelectableType.Building;
}
