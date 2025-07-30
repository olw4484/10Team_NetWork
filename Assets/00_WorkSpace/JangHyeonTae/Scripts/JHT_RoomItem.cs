using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JHT_RoomItem : YSJ_PanelBaseUI
{
    private Image secret => GetUI<Image>("Secret");
    private TextMeshProUGUI roomNameText => GetUI<TextMeshProUGUI>("RoomNameText");
    private TextMeshProUGUI playerCountText => GetUI<TextMeshProUGUI>("PlayerCountText");
    [SerializeField] private Button joinButton;

    private string roomName;
    public void Init(RoomInfo info)
    {
        roomName = info.Name;
        roomNameText.text = $"Room : {roomName}";
        playerCountText.text = info.MaxPlayers.ToString();
        secret.color = info.IsVisible ? Color.green : Color.red;
        joinButton.onClick.AddListener(JoinRoom);
        //StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForEndOfFrame();
        GetEvent("RoomPanelItem").Click += data =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinRoom(roomName);
            }
        };
    }


    public void JoinRoom()
    {
        if (PhotonNetwork.InLobby)
            PhotonNetwork.JoinRoom(roomName);

        joinButton.onClick.RemoveListener(JoinRoom);
    }
}
