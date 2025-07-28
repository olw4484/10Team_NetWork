using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KMS_MinionManualSpawner : MonoBehaviour
{
    public KMS_HQDataSO data;
    public Transform spawnPoint;
    public Transform target;
    public KMS_CommandPlayer player;

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
                KMS_MinionFactory.Instance.TrySpawnMinion(
                    capturedInfo.type,
                    spawnPoint.position,
                    target,
                    player
                );
            });
        }
    }
}

