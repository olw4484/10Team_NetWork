using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAutoSpawner : BaseMinionSpawner
{
    [Header("Auto Spawn Settings")]
    public List<MinionType> minionSequence = new List<MinionType> { MinionType.Melee, MinionType.Melee, MinionType.Ranged, MinionType.Ranged };
    public Transform spawnPoint;
    public int teamId;
    [SerializeField] public float spawnDelay = 0.5f;
    [SerializeField] public float spawnInterval = 50f;
    public WaypointGroup waypointGroup;

    private float timer = 0f;
    private bool isSpawning = false;

    private void Update()
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            return; // �����͸� �ڵ� ��ȯ

        timer += Time.deltaTime;

        if (timer >= spawnInterval && !isSpawning)
        {
            timer = 0f;
            StartCoroutine(SpawnSequence());
        }
    }

    private IEnumerator SpawnSequence()
    {
        isSpawning = true;
        foreach (var type in minionSequence)
        {
            MinionFactory.Instance.SpawnAutoMinion(type, spawnPoint.position, waypointGroup, teamId);
            yield return new WaitForSeconds(spawnDelay);
        }
        isSpawning = false;
    }
}
