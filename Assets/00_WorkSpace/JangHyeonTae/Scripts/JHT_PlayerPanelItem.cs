using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JHT_PlayerPanelItem : JHT_BaseUI
{
    private TextMeshProUGUI playerNameText => GetUI<TextMeshProUGUI>("PlayerNameText");
    private Image hostImage => GetUI<Image>("HostImage");
    private Button readyButton => GetUI<Button>("PlayerReadyButton");

    bool isRed;
    public void Init(Player player)
    {
        player.NickName = PhotonNetwork.LocalPlayer.ActorNumber.ToString(); //FirebaseManager.Auth.CurrentUser.DisplayName;
        Debug.Log($"{player.NickName}");
        playerNameText.text = player.NickName;
        hostImage.enabled = player.IsMasterClient;
        readyButton.interactable = player.IsLocal;

        if (!player.IsLocal)
            return;

        //TeamPropertyUpdate();
    }

    //public void TeamPropertyUpdate()
    //{
    //    ExitGames.Client.Photon.Hashtable playerTeamProperty = new();
    //    playerTeamProperty["IsRed"] = isRed;
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(playerTeamProperty);
    //}

}
