using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class KMS_MinionAutoSpawner : KMS_BaseMinionSpawner
{
    [Header("Auto Spawn Settings")]
    public List<MinionType> minionSequence = new List<MinionType> { MinionType.Melee, MinionType.Melee, MinionType.Ranged, MinionType.Ranged };
    public Transform spawnPoint;
    public Transform target;
    public float spawnDelay = 0.5f;
    public float spawnInterval = 30f;

    private int currentIndex = 0;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            StartCoroutine(SpawnSequence());
        }
    }

    private IEnumerator SpawnSequence()
    {
        foreach (var type in minionSequence)
        {
            SpawnFreeMinion(type, spawnPoint.position, target);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
