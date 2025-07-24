using UnityEngine;

public class KMS_HQ : MonoBehaviour
{
    [Header("HQ 설정 데이터")]
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
        // 자동 미니언 생성 타이밍 처리
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

    // 데미지 처리 (공격받을 경우)
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
        Debug.Log($"{gameObject.name} HQ 파괴됨!");
        EventManager.Instance.HQDestroyed(teamId); // 파괴된 팀 ID 전송
        Destroy(gameObject);
    }
}
