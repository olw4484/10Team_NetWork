using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CurrentState
{
    NotConnect,
    Lobby,
    InRoom,
    InGame
}

public class JHT_NetworkManager : MonoBehaviourPunCallbacks
{
    #region singleton
    private static JHT_NetworkManager networkInstance;
    public static JHT_NetworkManager NetworkInstance
    {
        get { return networkInstance;}
    }

    private void Awake()
    {
        if (networkInstance == null)
        {
            networkInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private Transform roomListParent;
    [SerializeField] private GameObject roomPanelPrefab;

    private CurrentState curPlayerState;
    private Dictionary<string, GameObject> currentRoomDic;

    public JHT_RoomManager roomManager;
    void Start()
    {
        currentRoomDic = new();
        PhotonNetwork.ConnectUsingSettings();
    }

    #region ConnectNetwork & Lobby
    public override void OnConnectedToMaster()
    {
        if (loadingPanel.activeSelf)
        {
            loadingPanel.SetActive(false);
        }
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinedLobby()
    {
        lobbyPanel.SetActive(true);
        StateCustomProperty(CurrentState.Lobby);
    }

    public override void OnCreatedRoom()
    {
        StateCustomProperty(CurrentState.InRoom);
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomManager.PlayerPanelSpawn();
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(newPlayer != PhotonNetwork.LocalPlayer)
            roomManager.PlayerPanelSpawn(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer != PhotonNetwork.LocalPlayer)
            roomManager.PlayerLeaveRoom(otherPlayer);
    }
    public override void OnMasterClientSwitched(Player newClientPlayer)
    {
        roomManager.PlayerPanelSpawn(newClientPlayer);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            //���� ����� ��
            if (roomInfo.RemovedFromList)
            {
                if (currentRoomDic.TryGetValue(roomInfo.Name, out GameObject obj))
                {
                    Destroy(obj);
                    currentRoomDic.Remove(roomInfo.Name);
                }

                continue;
            }
            
            //���� ���� ��
            if (currentRoomDic.ContainsKey(roomInfo.Name))
            {
                currentRoomDic[roomInfo.Name].GetComponent<JHT_RoomItem>().Init(roomInfo);
            }
            else
            {
                GameObject prefab = Instantiate(roomPanelPrefab);
                prefab.transform.SetParent(roomListParent);
                prefab.GetComponent<JHT_RoomItem>().Init(roomInfo);
                currentRoomDic.Add(roomInfo.Name, prefab);
            }
        }
    }

    #endregion

    #region CustomProperty

    // Player Network State
    public void StateCustomProperty(CurrentState _curPlayerState)
    {
        ExitGames.Client.Photon.Hashtable curState = new();
        curPlayerState = _curPlayerState;
        curState["CurState"] = curPlayerState; // �������� �ۼ�����
        PhotonNetwork.LocalPlayer.SetCustomProperties(curState); //�������� ���� ����
    }



    // ������Ƽ ��ȭ�� �������� ȣ��, ȣ���� �Ǿ���� ������ ������ ���
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        //���⼭ GetCustomProperties ����ϸ� ��, GetCustoomProperties�� Ŀ���� ������Ƽ �о�ö� ���
    }


    #endregion
}
