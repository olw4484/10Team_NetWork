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
    public string groupId; 
    [SerializeField] public float spawnDelay = 0.5f;
    [SerializeField] public float spawnInterval = 50f;

    private float timer = 0f;
    private bool isSpawning = false;
    private WaypointGroup waypointGroup;

    private void Start()
    {
        Debug.Log($"[AutoSpawner] groupId: '{groupId}'");

        waypointGroup = WaypointManager.Instance.GetWaypointGroup(groupId);
        if (waypointGroup == null)
            Debug.LogError($"[AutoSpawner] WaypointGroup not found for groupId: {groupId}");
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            return; // 마스터만 자동 소환

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
            MinionFactory.Instance.SpawnAutoMinion(type, spawnPoint.position, groupId, teamId);
            yield return new WaitForSeconds(spawnDelay);
        }
        isSpawning = false;
    }
}
