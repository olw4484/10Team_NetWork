using UnityEngine;

public class KMS_HQ : MonoBehaviour
{
    [Header("HQ ���� ������")]
    public KMS_HQDataSO data;
    public int teamId;

    private int currentHP;
    private float spawnTimer;

    private void Start()
    {
        currentHP = data.maxHP;
        spawnTimer = 0f;
    }

    private void Update()
    {
        // �ڵ� �̴Ͼ� ���� Ÿ�̹� ó��
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= data.autoSpawnInterval)
        {
            spawnTimer = 0f;

            foreach (var spawnPoint in data.autoSpawnPoints)
            {
                for (int i = 0; i < data.autoMinionCount; i++)
                {
                    SpawnMinion(spawnPoint.position);
                }
            }
        }
    }

    private void SpawnMinion(Vector3 spawnPosition)
    {
        Instantiate(data.minionPrefab, spawnPosition, Quaternion.identity);
    }

    // ������ ó�� (���ݹ��� ���)
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            OnDestroyed();
        }
    }

    private void OnDestroyed()
    {
        Debug.Log($"{gameObject.name} HQ �ı���!");
        EventManager.Instance.HQDestroyed(teamId); // �ı��� �� ID ����
        Destroy(gameObject);
    }
}
