using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHT_SamplePrefab : MonoBehaviourPun
{

    void Start()
    {
        Debug.Log($"JHT_SamplePrefab Start : photonView.ViewID : {photonView.ViewID}");
        Debug.Log($"JHT_SamplePrefab Start : PhotonNetwork.LocalPlayer.ActorNumber : {PhotonNetwork.LocalPlayer.ActorNumber}");

        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Team",out object value))
        {
            Debug.Log($"JHT_SamplePrefab Start : TeamColor : {PhotonNetwork.LocalPlayer.CustomProperties["Team"]}");
        }
    }

}
