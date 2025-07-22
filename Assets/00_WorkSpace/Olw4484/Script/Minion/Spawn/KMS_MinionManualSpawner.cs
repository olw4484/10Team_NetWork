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
        meleeButton?.onClick.AddListener(() => SpawnMinion(MinionType.Melee, spawnPoint.position, target));
        rangedButton?.onClick.AddListener(() => SpawnMinion(MinionType.Ranged, spawnPoint.position, target));
        eliteButton?.onClick.AddListener(() => SpawnMinion(MinionType.Elite, spawnPoint.position, target));
    }
}

