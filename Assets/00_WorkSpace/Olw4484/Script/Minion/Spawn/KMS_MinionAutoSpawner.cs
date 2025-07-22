using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class KMS_MinionAutoSpawner : KMS_BaseMinionSpawner
{
    [Header("Spawner Settings")]
    public Transform spawnPoint;
    public Transform target;

    public float spawnInterval = 50f; // ��ü �ֱ� �⺻ 50��
    public float spawnDelay = 0.5f;   // ���� �̴Ͼ� ���� ���� �⺻ 0.5��
    public List<KMS_SpawnBatch> spawnBatchList = new List<KMS_SpawnBatch>();

    private float timer;
    private bool isSpawning = false;

    private void Update()
    {
        if (isSpawning) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            StartCoroutine(SpawnBatchRoutine());
        }
    }

    private IEnumerator SpawnBatchRoutine()
    {
        isSpawning = true;

        foreach (var batch in spawnBatchList)
        {
            for (int i = 0; i < batch.count; i++)
            {
                SpawnMinion(batch.minionType, spawnPoint.position, target);
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        isSpawning = false;
    }
}
