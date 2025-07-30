using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionManualSpawner : MonoBehaviour
{
    public HQDataSO data;
    public Transform spawnPoint;
    public Transform target;
    public CommandPlayer player;
    public int teamId;

    public Transform uiButtonParent;
    public GameObject buttonPrefab;

    private void Start()
    {
        foreach (var info in data.manualSpawnList)
        {
            var btnGO = Instantiate(buttonPrefab, uiButtonParent);
            btnGO.GetComponentInChildren<Text>().text = info.type.ToString();

            var capturedInfo = info;

            btnGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                MinionFactory.Instance.TrySpawnMinion(
                    capturedInfo.type,
                    spawnPoint.position,
                    target,
                    player,
                    teamId
                );
            });
        }
    }
}

