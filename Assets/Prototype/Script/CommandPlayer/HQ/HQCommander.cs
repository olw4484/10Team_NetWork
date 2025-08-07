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

        // 특수 스킬 확장 가능성 ) HQSkil 2~3개
    }

    public void OnSpawnMinionButton(int type)
    {
        var minionType = (MinionType)type;
        var spawnPos = defaultSpawnPoint.position;

        // NavMesh 위로 위치 보정
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 3f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
            Debug.Log($"[HQCommander] NavMesh 스폰 성공: 위치={spawnPos}");
        }
        else
        {
            Debug.LogError("[HQCommander] NavMesh 위에서 미니언을 스폰할 수 없습니다! 스폰 포인트 위치 재확인 필요");
            return;
        }

        var target = rallyPointTarget != null ? rallyPointTarget : null;

        if (player == null)
        {
            Debug.LogError("[HQCommander] player가 null임");
            return;
        }
        if (MinionFactory.Instance == null)
        {
            Debug.LogError("[HQCommander] MinionFactory.Instance가 null임");
            return;
        }
        if (MinionFactory.Instance.hqData == null)
        {
            Debug.LogError("[HQCommander] MinionFactory.Instance.hqData가 null임");
            return;
        }
        if (MinionFactory.Instance.hqData.manualSpawnList == null)
        {
            Debug.LogError("[HQCommander] manualSpawnList가 null임");
            return;
        }

        int teamId = player.teamId;

        Debug.Log($"[HQCommander] TrySpawnMinion 호출: minionType={minionType}, teamId={teamId}");
        bool result = MinionFactory.Instance.TrySpawnMinion(minionType, spawnPos, target, player, teamId);

        if (!result)
            Debug.LogError("[HQCommander] TrySpawnMinion 실패");
        else
            Debug.Log("[HQCommander] TrySpawnMinion 성공");
    }

    public void SetRallyPoint(Vector3 point)
    {
        rallyPointTarget.position = point;

        if (rallyPointMarker != null)
        {
            rallyPointMarker.transform.position = point;
            rallyPointMarker.SetActive(true);
        }

        Debug.Log($"[Commander] Rally Point 설정: {point}");
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            teamId = (int)data[0];
            Debug.Log($"[HQCommander] teamId 동기화: {teamId}");
        }
    }

    public void Select()
    {
        IsSelected = true;
        // UI 표시 기능 등록
    }

    public void Deselect()
    {
        IsSelected = false;
        // UI 표시 기능 해제
    }

    public SelectableType GetSelectableType() => SelectableType.Building;
}
