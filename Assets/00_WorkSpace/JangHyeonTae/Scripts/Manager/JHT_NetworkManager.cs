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
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private Transform roomListParent;
    [SerializeField] private GameObject roomPanelPrefab;

    private CurrentState curPlayerState;
    private Dictionary<string, GameObject> currentRoomDic;

    public JHT_RoomManager roomManager;
    public JHT_TeamManager teamManager;

    void Start()
    {
        currentRoomDic = new(); 
        PhotonNetwork.NickName = FirebaseManager.Auth.CurrentUser.DisplayName;
        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Callbacks
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
        StateCustomProperty(CurrentState.NotConnect);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinedLobby()
    {
        lobbyPanel.SetActive(true);
        StateCustomProperty(CurrentState.Lobby);
    }

    public override void OnCreatedRoom()
    {
        ExitGames.Client.Photon.Hashtable roomInit = new();
        roomInit["RedCount"] = 0;
        roomInit["BlueCount"] = 0;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomInit);

    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        StartCoroutine(CallPlayer());
    }

    private IEnumerator CallPlayer()
    {
        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RedCount") ||
           !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BlueCount")) 
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        if (PhotonNetwork.IsMasterClient)
            teamManager.SetPlayerTeam(PhotonNetwork.LocalPlayer);

        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            yield return null;
        }

        roomManager.PlayerPanelSpawn();
        StateCustomProperty(CurrentState.InRoom);
    }

    public override void OnLeftRoom()
    {
        StateCustomProperty(CurrentState.Lobby);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer != PhotonNetwork.LocalPlayer)
        {
            StartCoroutine(CallOtherPlayer(newPlayer));
        }
    }

    private IEnumerator CallOtherPlayer(Player newPlayer)
    {
        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RedCount") ||
           !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BlueCount"))
        {
            yield return null;
        }

        if (PhotonNetwork.IsMasterClient)
            teamManager.SetPlayerTeam(newPlayer);

        while (!newPlayer.CustomProperties.ContainsKey("Team"))
            yield return null;

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer != PhotonNetwork.LocalPlayer)
        {
            roomManager.PlayerLeaveRoom(otherPlayer);
        }
    }
    public override void OnMasterClientSwitched(Player newClientPlayer)
    {
        roomManager.PlayerPanelSpawn(newClientPlayer);
        Debug.Log($"마스터 클라이언트 변경 : {newClientPlayer.ActorNumber}");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            //방이 사라질 때
            if (roomInfo.RemovedFromList)
            {
                if (currentRoomDic.TryGetValue(roomInfo.Name, out GameObject obj))
                {
                    Destroy(obj);
                    currentRoomDic.Remove(roomInfo.Name);
                }

                continue;
            }
            
            //방이 생길 때
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
        curState["CurState"] = curPlayerState; // 위에까지 작성과정
        PhotonNetwork.LocalPlayer.SetCustomProperties(curState); //전역변수 저장 느낌
    }

    // 프로퍼티 변화가 생겼을때 호출, 호출이 되어야할 로직이 있을 사용
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("RedCount"))
            teamManager.redCount = (int)propertiesThatChanged["RedCount"];
        if (propertiesThatChanged.ContainsKey("BlueCount"))
            teamManager.blueCount = (int)propertiesThatChanged["BlueCount"];
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Team"))
        {
            roomManager.OtherPlayerChangeTeam(targetPlayer);
        }
        else if(changedProps.ContainsKey("IsReady"))
        {
            StartCoroutine(WaitForAddDic(targetPlayer, changedProps));
        }
        else if(changedProps.ContainsKey("Character"))
        {
            StartCoroutine(WaitForLoadCharacter(targetPlayer, changedProps));
        }
          
    }

    IEnumerator WaitForAddDic(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        while (!roomManager.playerPanelDic.ContainsKey(targetPlayer.ActorNumber))
            yield return null;


        if (changedProps.ContainsKey("IsReady"))
        {
            if (roomManager.playerPanelDic.TryGetValue(targetPlayer.ActorNumber, out var panel))
            {
                panel.CheckReady(targetPlayer);
            }
            else
            {
                Debug.LogWarning($"[IsReady] 패널 없음: {targetPlayer.ActorNumber}");
            }
        }
    }

    IEnumerator WaitForLoadCharacter(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        while (!roomManager.playerPanelDic.ContainsKey(targetPlayer.ActorNumber))
            yield return null;

        Debug.Log($"WaitForLoadCharacter : {targetPlayer.ActorNumber}");

        if (changedProps.ContainsKey("Character"))
        {
            if (roomManager.playerPanelDic.TryGetValue(targetPlayer.ActorNumber, out var panel))
            {
                panel.SetChangeCharacter(targetPlayer);
            }
            else
            {
                Debug.LogWarning($"[Character] 패널 없음: {targetPlayer.ActorNumber}");
            }
        }
    }
    #endregion
}
