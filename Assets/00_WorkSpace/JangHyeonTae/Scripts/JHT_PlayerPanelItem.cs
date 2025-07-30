using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JHT_PlayerPanelItem : YSJ_PanelBaseUI
{
    private TextMeshProUGUI playerNameText => GetUI<TextMeshProUGUI>("PlayerNameText");
    private Image hostImage => GetUI<Image>("HostImage");
    private Image playerCharacterImage => GetUI<Image>("PlayerCharacterImage");
    private Image readyButtonImage => GetUI<Image>("PlayerReadyButton");

    private TextMeshProUGUI readyText => GetUI<TextMeshProUGUI>("PlayerReadyText");
    private Button readyButton => GetUI<Button>("PlayerReadyButton");


    TeamSetting team;
    private bool isReady;
    public void Init(Player player)
    {
        PhotonNetwork.LocalPlayer.NickName = PhotonNetwork.LocalPlayer.ActorNumber.ToString(); //FirebaseManager.Auth.CurrentUser.DisplayName;
        player.NickName = PhotonNetwork.LocalPlayer.NickName;
        playerNameText.text = player.NickName;
        hostImage.enabled = player.IsMasterClient;
        readyButton.interactable = player.IsLocal;

        if (!player.IsLocal)
            return;

        isReady = false;
        SetReadyProperty();

        readyButton.onClick.RemoveListener(ReadyButtonClick);
        readyButton.onClick.AddListener(ReadyButtonClick);
    }

    public void ReadyButtonClick()
    {
        isReady = !isReady;

        readyText.text = isReady ? "Ready" : "Waiting";
        readyButtonImage.color = isReady ? Color.yellow : Color.gray;
        SetReadyProperty();
    }

    public void SetReadyProperty()
    {
        ExitGames.Client.Photon.Hashtable readyCustom = new();
        readyCustom["IsReady"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(readyCustom);
    }

    public void CheckReady(Player player)
    {
        if (player.CustomProperties.TryGetValue("IsReady", out object value))
        {
            readyText.text = (bool)value ? "Ready" : "Waiting";
            readyButtonImage.color = (bool)value ? Color.yellow : Color.gray;
        }
    }


}
