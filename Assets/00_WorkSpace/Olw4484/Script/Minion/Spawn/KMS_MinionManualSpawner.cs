using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KMS_MinionManualSpawner : KMS_BaseMinionSpawner
{
    [Header("Spawner Settings")]
    public Transform spawnPoint;
    public Transform target;

    [Header("UI Buttons")]
    public Button meleeButton;
    public Button rangedButton;
    public Button eliteButton;

    private void Start()
    {
        meleeButton?.onClick.AddListener(() => TrySpawn(MinionType.Melee));
        rangedButton?.onClick.AddListener(() => TrySpawn(MinionType.Ranged));
        eliteButton?.onClick.AddListener(() => TrySpawn(MinionType.Elite));
    }

    private void TrySpawn(MinionType type)
    {
        KMS_MinionFactory.Instance.TrySpawnMinion(type, spawnPoint.position, target);
    }
}

