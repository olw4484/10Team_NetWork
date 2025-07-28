using UnityEngine;
using static KMS_ISelectable;
using static UnityEngine.GraphicsBuffer;

public class KMS_HQCommander : MonoBehaviour, KMS_ISelectable 
{
    [Header("연동 대상")]
    public KMS_CommandPlayer player;

    [Header("소환 위치 및 타겟")]
    public Transform defaultSpawnPoint;
    public Transform rallyPointTarget;
    public GameObject rallyPointMarker;

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
        var target = rallyPointTarget != null ? rallyPointTarget : null;
        KMS_MinionFactory.Instance.TrySpawnMinion(minionType, spawnPos, target, player);
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
