using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JHT_RoomItem : JHT_BaseUI
{
    private Image secret => GetUI<Image>("Secret");
    private TextMeshProUGUI roomNameText => GetUI<TextMeshProUGUI>("RoomNameText");
    private TextMeshProUGUI playerCountText => GetUI<TextMeshProUGUI>("PlayerCountText");
    public void Init(RoomInfo info)
    {
        roomNameText.text = info.Name;
        playerCountText.text = info.MaxPlayers.ToString();
        secret.color = info.IsVisible ? Color.green : Color.red;

        GetEvent("RoomPanelItem").Click += data =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinRoom(roomNameText.text);
            }
        };
    }

}
