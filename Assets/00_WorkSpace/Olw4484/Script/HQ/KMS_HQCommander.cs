using UnityEngine;

public class KMS_HQCommander : MonoBehaviour
{
    [Header("���� ���")]
    public KMS_CommandPlayer player;

    [Header("��ȯ ��ġ �� Ÿ��")]
    public Transform defaultSpawnPoint;
    public Transform rallyPointTarget;
    public GameObject rallyPointMarker;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            TrySpawn(MinionType.Melee);

        if (Input.GetKeyDown(KeyCode.W))
            TrySpawn(MinionType.Ranged);

        if (Input.GetKeyDown(KeyCode.E))
            TrySpawn(MinionType.Elite);

        // ���� Ȯ��: D, F Ű Ư�� ��� ��

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                SetRallyPoint(hit.point);
            }
        }
    }

    private void TrySpawn(MinionType type)
    {
        var spawnPos = defaultSpawnPoint.position;
        var target = rallyPointTarget != null ? rallyPointTarget : null;

        KMS_MinionFactory.Instance.TrySpawnMinion(type, spawnPos, target, player);
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
}
