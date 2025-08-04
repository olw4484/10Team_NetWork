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
    #region 로비
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

    #region 룸
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

    #region 툴팁

    GameObject tool1 => GetUI("DescPopUp1");
    GameObject tool2 => GetUI("DescPopUp2");
    GameObject tool3 => GetUI("DescPopUp3");

    #endregion

    private bool isSecret;

    private JHT_RoomManager roomManager;     // Manager가 아닌 스크립트도 ManagerGroup.Instance.GetManager<JHT_RoomManager>()로 접급해야하는지
    private JHT_TeamManager teamManager;     // Manager가 아닌 스크립트도 ManagerGroup.Instance.GetManager<JHT_TeamManager>()로 접급해야하는지

    #region NetworkManager
    private GameObject loadingPanel => GetUI("LoadingPanel");
    
    private RectTransform roomListParent => GetUI<RectTransform>("RoomPanelItemParent");
    
    private JHT_NetworkManager networkManager;
    #endregion

    #region RoomManager
    private RectTransform playerRedPanelParent => GetUI<RectTransform>("PlayerListRedParent");
    private RectTransform playerBluePanelParent => GetUI<RectTransform>("PlayerListBlueParent");
    #endregion

    #region TeamManager

    public void TeamInit()
    {
        teamManager = ManagerGroup.Instance.GetManager<JHT_TeamManager>();


        #region 팀 들어가고 나가기
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
            Debug.Log($"{PhotonNetwork.LocalPlayer.ActorNumber} 블루버튼 클릭");
        };
        #endregion
    }
    #endregion
    public void NetInit()
    {
        networkManager = ManagerGroup.Instance.GetManager<JHT_NetworkManager>();

        if (networkManager == null)
            networkManager = FindObjectOfType<JHT_NetworkManager>();

        networkManager.OnLoading += AddLoading;
        networkManager.OnLobbyIn += AddLobby;
        networkManager.OnRoomIn += AddRoom;
        networkManager.OnParent += AddParent;
        #region 룸 버튼 이벤트
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

    public void RoomInit()
    {
        roomManager = ManagerGroup.Instance.GetManager<JHT_RoomManager>();


        if (roomManager == null)
        {
            roomManager = FindObjectOfType<JHT_RoomManager>();
        }

        roomManager.OnStartButtonActive += StartButtonActive;
        roomManager.OnSetRedParent += RedParent;
        roomManager.OnSetBlueParent += BlueParent;

        GetEvent("StartButton").Click += data =>
        {
            roomManager.GameStart();
        };


        GetEvent("LeaveRoomButton").Click += data =>
        {
            roomPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            roomManager.OnLeaveRoom?.Invoke();
        };

    }

    #region RoomManager Event

    private void StartButtonActive(bool value)
    {
        startButton.interactable = value;
    }

    private RectTransform RedParent()
    {
        return playerRedPanelParent;
    }

    private RectTransform BlueParent()
    {
        return playerBluePanelParent;
    }

    #endregion 

    #region NetworkManager Event
    private void AddLoading(bool value)
    {
        if (this == null || gameObject == null) return;

        if (loadingPanel.activeSelf)
            loadingPanel.SetActive(value);
            
    }

    private void AddLobby(bool value)
    {
        if (this == null || gameObject == null) return;

        lobbyPanel.SetActive(value);
    }

    private void AddRoom(bool value, bool value2 = false)
    {
        if (this == null || gameObject == null) return;

        roomPanel.SetActive(value);
        if (value2)
        {
            lobbyPanel.SetActive(!value2);
        }
    }

    private RectTransform AddParent()
    {
        return roomListParent;
    }
    #endregion

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
