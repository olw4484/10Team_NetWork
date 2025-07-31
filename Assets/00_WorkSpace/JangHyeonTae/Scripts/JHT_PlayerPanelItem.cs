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
    private int curCharacterIndex;

    public JHT_NetworkUIPanel networkUICanvas;
    private string curMyCharacter;

    public TeamSetting myTeam;
    public void Init(Player player)
    {
        playerNameText.text = player.NickName;
        hostImage.enabled = player.IsMasterClient;
        readyButton.interactable = player.IsLocal;


        if (!player.IsLocal)
            return;

        networkUICanvas = FindObjectOfType<JHT_NetworkUIPanel>();
        isReady = false;
        SetReadyProperty();
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
        if (networkUICanvas.curIndex < 0)
            return;

        curCharacterIndex = networkUICanvas.curIndex;
        SetCharacterProperty();
    }

    public void SetCharacterProperty()
    {
        ExitGames.Client.Photon.Hashtable characterChanged = new();
        characterChanged["Character"] = curCharacterIndex;
        PhotonNetwork.LocalPlayer.SetCustomProperties(characterChanged);
    }

    public void SetChangeCharacter(Player player)
    {
        if (networkUICanvas == null)
        {
            Debug.LogWarning("networkUICanvas Findobject로 못찾음");
            networkUICanvas = FindObjectOfType<JHT_NetworkUIPanel>();
            if (networkUICanvas == null)
            {
                Debug.LogError("networkUICanvas Findobject로 다시 찾아도 못찾음");
                return;
            }
        }

        if (player.CustomProperties.TryGetValue("Character", out object value))
        {
            playerCharacterImage.sprite = JHT_NetworkManager.NetworkInstance.characters[(int)value].icon;
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

    #endregion

    
}
