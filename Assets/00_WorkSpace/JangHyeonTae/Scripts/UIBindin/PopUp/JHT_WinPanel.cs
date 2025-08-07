using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JHT_WinPanel : YSJ_PopupBaseUI
{
    private TextMeshProUGUI winLoseText => GetUI<TextMeshProUGUI>("WinLoseText");
    private Image winLoseImage => GetUI<Image>("WinLoseImage");
    private Image winLosePanel => GetUI<Image>("WinLosePanel");

    private void Start()
    {
        //if (ManagerGroup.Instance.GetManager<LGH_TestGameManager>().IsWin)
        //{
        //    winLoseText.text = "You Win!";
        //    winLoseImage.color = Color.blue;
        //    winLosePanel.color = Color.blue;
        //}
        //else
        //{
        //    winLoseText.text = "You Lose!";
        //    winLoseImage.color = Color.red;
        //    winLosePanel.color = Color.red;
        //}

        GetEvent("LobbyButton").Click += data =>
        {
            LeaveRoom();
        };
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object value))
        {
            if ((TeamSetting)value == TeamSetting.Blue || (TeamSetting)value == TeamSetting.Red)
            {
                ExitGames.Client.Photon.Hashtable props = new();
                props["Team"] = null;
                props["Role"] = null;
                props["Character"] = -1;
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }

        PhotonNetwork.LeaveRoom();
    }
}
