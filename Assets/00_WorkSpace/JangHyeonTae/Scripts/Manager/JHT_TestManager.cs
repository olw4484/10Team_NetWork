using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHT_TestManager : MonoBehaviour
{
    public void Start()
    {
        ExitGames.Client.Photon.Hashtable props = new();
        props["Team"] = TeamSetting.Red;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        
        
    }

    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            
        }
    }

}
