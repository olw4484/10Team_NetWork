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


    private bool isReady;
    public int curCharacterIndex { get; set; }

    public JHT_NetworkUIPanel networkUICanvas;
    private string curMyCharacter;

    public TeamSetting myTeam;
    public void Init(Player player)
    {
        playerNameText.text = player.NickName;
        hostImage.enabled = player.IsMasterClient;
        readyButton.interactable = player.IsLocal;
        curCharacterIndex = -1;

        if (!player.IsLocal)
            return;

        networkUICanvas = FindObjectOfType<JHT_NetworkUIPanel>();
        isReady = false;
        SetReadyProperty();
        networkUICanvas.OnChangedClick -= SetCharacter;
        networkUICanvas.OnChangedClick += SetCharacter;

        readyButton.onClick.RemoveListener(ReadyButtonClick);
        readyButton.onClick.AddListener(ReadyButtonClick);
    }

    private void OnDestroy()
    {
        if (networkUICanvas != null)
            networkUICanvas.OnChangedClick -= SetCharacter;
    }

    #region Character select

    public void SetCharacter()
    {
        if (networkUICanvas.curIndex == -1)
        {
            return;
        }
        Debug.Log($"PlayerPanelItem - {curCharacterIndex}");
        curCharacterIndex = networkUICanvas.curIndex;
        SetCharacterProperty();
    }

    public void SetLeaveCharacter()
    {
        curCharacterIndex = -1;
        SetCharacterProperty();
    }

    public void SetChangeTeamCharacter(Player player)
    {
        if ((int)player.CustomProperties["HeroIndex"] == -1)
        {
            return;
        }

        SetCharacterProperty();
    }

    public void SetCharacterProperty()
    {
        ExitGames.Client.Photon.Hashtable characterChanged = new();
        characterChanged["HeroIndex"] = curCharacterIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(characterChanged);
    }

    public void SetChangeCharacter(Player player)
    {
        if (networkUICanvas == null)
        {
            networkUICanvas = FindObjectOfType<JHT_NetworkUIPanel>();
            if (networkUICanvas == null)
            {
                Debug.LogError("networkUICanvas Findobject로 다시 찾아도 못찾음");
                return;
            }
        }

        if (player.CustomProperties.TryGetValue("HeroIndex", out object value))
        {
            if ((int)value < 0)
                return;

            playerCharacterImage.sprite = ManagerGroup.Instance.GetManager<JHT_NetworkManager>().characters[(int)value].icon;
        }
    }
    #endregion


    #region readyButton
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

    public void SetLeaveReady()
    {
        if (isReady)
        {
            ReadyButtonClick();
        }
    }

    #endregion


}
