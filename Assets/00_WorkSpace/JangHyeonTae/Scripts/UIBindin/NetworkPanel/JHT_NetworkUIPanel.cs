using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class JHT_NetworkUIPanel : YSJ_PanelBaseUI
{
    #region ·Îºñ
    private GameObject lobbyPanel => GetUI("LobbyPanel");
    private GameObject createRoomPanel => GetUI("CreateRoomPanel");
    private GameObject roomPanelItem => GetUI("RoomPanelItemParent");
    private Button createLobbyButton => GetUI<Button>("CreateLobbyButton");
    private Button leaveLobbyButton => GetUI<Button>("LeaveLobbyButton");
    private Button createRoomButton => GetUI<Button>("CreateRoomButton");
    private Button secretButton => GetUI<Button>("SecretButton");
    private Button backButton => GetUI<Button>("BackButton");
    private TMP_InputField roomNameInput => GetUI<TMP_InputField>("RoomNameInput");
    private Image secretImage => GetUI<Image>("SecretButton");

    #endregion

    #region ·ë
    private GameObject roomPanel => GetUI("RoomPanel");
    private Button leaveRoomButton => GetUI<Button>("LeaveRoomButton");
    private Button startButton => GetUI<Button>("StartButton");
    private Image redTeamPanel => GetUI<Image>("RedTeamPanel");
    private Image blueTeamPanel => GetUI<Image>("BlueTeamPanel");

    #endregion

    #region Character Select

    [SerializeField] private GameObject descPopUp;

    Color normalColor = Color.white;

    public Action OnChangedClick;
    public int curIndex = -1;
    #endregion

    #region ÅøÆÁ

    GameObject tool1 => GetUI("DescPopUp1");
    GameObject tool2 => GetUI("DescPopUp2");
    GameObject tool3 => GetUI("DescPopUp3");

    #endregion

    private bool isSecret;

    [SerializeField] private JHT_RoomManager roomManager;
    [SerializeField] private JHT_TeamManager teamManager;
    private void Start()
    {
        #region ·ë ¹öÆ° ÀÌº¥Æ®
        GetEvent("CreateLobbyButton").Click += data =>
        {
            createRoomPanel.SetActive(true);
            roomPanelItem.SetActive(false);
            createLobbyButton.interactable = false;
            leaveLobbyButton.interactable = false;
        };

        GetEvent("SecretButton").Click += data =>
        {
            StartCoroutine(ButtonColorChange());
        };

        GetEvent("CreateRoomButton").Click += data =>
        {
            if (string.IsNullOrEmpty(roomNameInput.text))
            {
                //JHT_UIManager.UIInstance.ShowPopUp<JHT_ErrorText>();
                return;
            }

            createRoomButton.interactable = false;
            roomPanelItem.SetActive(true);
            createRoomPanel.SetActive(false);

            RoomOptions options = new RoomOptions();
            int maxPlayer = 4;
            options.IsVisible = !isSecret;
            options.MaxPlayers = maxPlayer;

            PhotonNetwork.CreateRoom(roomNameInput.text, options);

            roomPanel.SetActive(true);
            createLobbyButton.interactable = true;
            createRoomButton.interactable = true;
            leaveLobbyButton.interactable = true;
            lobbyPanel.SetActive(false);
        };

        GetEvent("BackButton").Click += data =>
        {
            if (!createRoomPanel.activeSelf)
                return;

            createLobbyButton.interactable = true;
            leaveLobbyButton.interactable = true;
            roomPanelItem.SetActive(true);
            createRoomPanel.SetActive(false);
        };

        GetEvent("LeaveRoomButton").Click += data =>
        {
            roomPanel.SetActive(false);
            lobbyPanel.SetActive(true);
        };
        #endregion


        #region ÆÀ µé¾î°¡°í ³ª°¡±â
        Color redBasicColor = redTeamPanel.color;
        Color blueBasicColor = blueTeamPanel.color;

        GetEvent("RedTeamPanel").Enter += data => redTeamPanel.color = new Color(redTeamPanel.color.r, redTeamPanel.color.g, redTeamPanel.color.b, 0.4f);
        GetEvent("RedTeamPanel").Exit += data => redTeamPanel.color = redBasicColor;

        GetEvent("BlueTeamPanel").Enter += data => blueTeamPanel.color = new Color(blueTeamPanel.color.r, blueTeamPanel.color.g, blueTeamPanel.color.b, 0.4f);
        GetEvent("BlueTeamPanel").Exit += data => blueTeamPanel.color = blueBasicColor;


        GetEvent("RedTeamPanel").Click += data =>
        {
            teamManager.OnRedSelect?.Invoke(PhotonNetwork.LocalPlayer);

        };

        GetEvent("BlueTeamPanel").Click += data =>
        {
            teamManager.OnBlueSelect?.Invoke(PhotonNetwork.LocalPlayer);

        };
        #endregion
        

        #region Character select
        for (int i = 0; i < JHT_NetworkManager.NetworkInstance.characters.Length; i++)
        {
            ChangeClick();
            GetUI<Image>($"CharacterPanel{i + 1}").sprite = JHT_NetworkManager.NetworkInstance.characters[i].icon;
        }

        GetEvent("CharacterPanel1").Click += data =>
        {
            ChangeClick();
            GetUI<Image>("CharacterPanel1").color = Color.yellow;
            curIndex = 0;
            OnChangedClick?.Invoke();
            //YSJ_GameManager.Instance.playerName = character[0].name;
        };
        GetEvent("CharacterPanel2").Click += data =>
        {
            ChangeClick();
            GetUI<Image>("CharacterPanel2").color = Color.yellow;
            curIndex = 1;
            OnChangedClick?.Invoke();
        };
        GetEvent("CharacterPanel3").Click += data =>
        {
            ChangeClick();
            GetUI<Image>("CharacterPanel3").color = Color.yellow;
            curIndex = 2;
            OnChangedClick?.Invoke();
        };
        #endregion


        #region ToolTip for Character
        for (int i = 0; i < JHT_NetworkManager.NetworkInstance.characters.Length; i++)
        {
            GetUI($"DescPopUp{i + 1}").SetActive(false);
        }

        GetEvent("CharacterPanel1").Enter += data =>
        {
            GetUI($"DescPopUp1").SetActive(true);
            GetUI<JHT_DescPopUp>("DescPopUp1").Init(JHT_NetworkManager.NetworkInstance.characters[0].desc);
        };

        GetEvent("CharacterPanel1").Exit += data =>
        {
            GetUI($"DescPopUp1").SetActive(false);
        };

        GetEvent("CharacterPanel2").Enter += data =>
        {
            GetUI($"DescPopUp2").SetActive(true);
            GetUI<JHT_DescPopUp>("DescPopUp2").Init(JHT_NetworkManager.NetworkInstance.characters[1].desc);
        };

        GetEvent("CharacterPanel2").Exit += data =>
        {
            GetUI($"DescPopUp2").SetActive(false);
        };

        GetEvent("CharacterPanel3").Enter += data =>
        {
            GetUI($"DescPopUp3").SetActive(true);
            GetUI<JHT_DescPopUp>("DescPopUp3").Init(JHT_NetworkManager.NetworkInstance.characters[2].desc);
        };

        GetEvent("CharacterPanel3").Exit += data =>
        {
            GetUI($"DescPopUp3").SetActive(false);
        };
        #endregion
    }


    private IEnumerator ButtonColorChange()
    {
        isSecret = !isSecret;
        yield return new WaitForSeconds(0.2f);
        secretImage.color = isSecret ? Color.red : Color.green;
    }

    private void ChangeClick()
    {
        for (int i = 0; i < JHT_NetworkManager.NetworkInstance.characters.Length; i++)
        {
            GetUI<Image>($"CharacterPanel{i + 1}").color = normalColor;
        }
    }
}
