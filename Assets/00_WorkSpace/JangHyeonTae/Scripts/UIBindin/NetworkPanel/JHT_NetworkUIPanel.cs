using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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


    private bool isSecret;

    [SerializeField] private JHT_RoomManager roomManager;
    [SerializeField] private JHT_TeamManager teamManager;
    private void Start()
    {
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
    }


    private IEnumerator ButtonColorChange()
    {
        isSecret = !isSecret;
        yield return new WaitForSeconds(0.2f);
        secretImage.color = isSecret ? Color.red : Color.green;
    }

}
