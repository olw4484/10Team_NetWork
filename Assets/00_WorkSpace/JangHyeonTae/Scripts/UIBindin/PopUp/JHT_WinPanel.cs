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
    public override void Open()
    {
        base.Open();

    }

    public void Init(bool isWin)
    {
        TextMeshProUGUI winText = GetUI<TextMeshProUGUI>("WinLoseText");
        Image winImage = GetUI<Image>("WinLoseImage");
        Image winPanel = GetUI<Image>("WinLosePanel");

        if (winText == null)
        {
            Debug.LogError($"[WinPanel] UI 찾기 실패 - WinLoseText:{(winText == null)}");
            return;
        }
        if (winImage == null)
        {
            Debug.LogError($"[WinPanel] UI 찾기 실패 - WinLoseImage:{(winImage == null)}");
        }

        if (winPanel == null)
        {
            Debug.LogError($"[WinPanel] UI 찾기 실패 -   WinLosePanel:{(winPanel == null)}");
        }

        if (isWin)
        {
            winText.text = "You Win!";
            winImage.color = Color.green;
            winPanel.color = Color.green;
        }
        else
        {
            winText.text = "You Lose!";
            winImage.color = Color.red;
            winPanel.color = Color.red;
        }

        GetEvent("LobbyButton").Click += data =>
        {
            LeaveRoom();
        };
    }

    public void LeaveRoom()
    {
        Debug.Log("LeaveRoom----------------------------------------------------------");
        ManagerGroup.Instance.GetManager<YSJ_SystemManager>().ResumeGame();
        
        Destroy(this.gameObject);

        ManagerGroup.Instance.GetManager<YSJ_SystemManager>().LoadSceneWithPreActions("Lobby");
        PhotonNetwork.LeaveRoom();
    }

}
