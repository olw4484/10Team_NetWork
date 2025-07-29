using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KMS_MinionAutoSpawner : KMS_BaseMinionSpawner
{
    [Header("Auto Spawn Settings")]
    public List<MinionType> minionSequence = new List<MinionType> { MinionType.Melee, MinionType.Melee, MinionType.Ranged, MinionType.Ranged };
    public Transform spawnPoint;
    public Transform target;
    [SerializeField] public float spawnDelay = 0.5f;
    [SerializeField] public float spawnInterval = 50f;
    public KMS_WaypointGroup waypointGroup;

    private int currentIndex = 0;
    private float timer = 0f;

    private void Update()
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            return; // 마스터만 자동 소환

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
            SpawnFreeMinion(type, spawnPoint.position, target, waypointGroup);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
