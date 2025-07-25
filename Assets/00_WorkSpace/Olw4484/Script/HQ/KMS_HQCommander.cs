using UnityEngine;
using static KMS_ISelectable;
using static UnityEngine.GraphicsBuffer;

public class KMS_HQCommander : MonoBehaviour, KMS_ISelectable 
{
    [Header("���� ���")]
    public KMS_CommandPlayer player;

    [Header("��ȯ ��ġ �� Ÿ��")]
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

        // Ư�� ��ų Ȯ�� ���ɼ� ) HQSkil 2~3��
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

        Debug.Log($"[Commander] Rally Point ����: {point}");
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
