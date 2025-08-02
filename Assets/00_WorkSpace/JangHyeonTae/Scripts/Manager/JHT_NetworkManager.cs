using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CurrentState
{
    NotConnect,
    Lobby,
    InRoom,
    InGame
}

public class JHT_NetworkManager : MonoBehaviourPunCallbacks, IManager
{

    #region singleton
    private static JHT_NetworkManager networkInstance;
    public static JHT_NetworkManager NetworkInstance
    {
        get
        {
            if (networkInstance == null)
            {
                var obj = FindObjectOfType<JHT_NetworkManager>();
                if (obj != null)
                {
                    networkInstance = obj;
                }
                else
                {
                    var newObj = new GameObject().AddComponent<JHT_NetworkManager>();
                    networkInstance = newObj;
                }
            }
            return networkInstance;
        }
    }

    #endregion

    #region 변수
    
    [SerializeField] private GameObject roomPanelPrefab;
    
    public CurrentState curPlayerState;
    private Dictionary<string, GameObject> currentRoomDic;
    [Header("캐릭터")]
    public JHT_Character[] characters;
    public JHT_NetworkUIPanel mainLobbyPanel;
    #endregion

    #region IManager

    public bool IsDontDestroy => true;

    public Action<bool> OnLoading;
    public Action<bool> OnLobbyIn;
    public Action<bool,bool> OnRoomIn;
    public Func<RectTransform> OnParent;

    public void Initialize()
    {
        var objs = FindObjectsOfType<JHT_NetworkManager>();
        if (objs.Length != 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Init();
    }

    public void Cleanup() { }

    public GameObject GetGameObject() => this.gameObject;
    #endregion

    public void Init()
    {
        currentRoomDic = new();
        PhotonNetwork.ConnectUsingSettings();
        mainLobbyPanel = Instantiate(mainLobbyPanel);
        mainLobbyPanel.TeamInit();
        mainLobbyPanel.NetInit();
        mainLobbyPanel.RoomInit();
        //PhotonNetwork.NickName = FirebaseManager.Auth.CurrentUser.DisplayName;
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        OnLoading?.Invoke(false);
        PhotonNetwork.JoinLobby();
        Debug.Log("마스터서버 접속");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        StateCustomProperty(CurrentState.NotConnect);
        if ((CurrentState)PhotonNetwork.LocalPlayer.CustomProperties["CurState"] == CurrentState.NotConnect)
            PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinedLobby()
    {
        OnLobbyIn?.Invoke(true);
        StateCustomProperty(CurrentState.Lobby);
    }

    public override void OnCreatedRoom()
    {
        ExitGames.Client.Photon.Hashtable roomInit = new();
        roomInit["RedCount"] = 0;
        roomInit["BlueCount"] = 0;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomInit);
        //ManagerGroup.Instance.GetManager<JHT_TeamManager>().SetTeamCount(0, 0);
    }


    public override void OnJoinedRoom()
    {
        StartCoroutine(CallPlayer());
    }

    private IEnumerator CallPlayer()
    {
        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RedCount") ||
           !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BlueCount")) 
        {
            yield return null;
        }


        if (PhotonNetwork.IsMasterClient)
            ManagerGroup.Instance.GetManager<JHT_TeamManager>().SetPlayerTeam(PhotonNetwork.LocalPlayer);

        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
        {
            yield return null;
        }

        ManagerGroup.Instance.GetManager<JHT_RoomManager>().PlayerPanelSpawn();
        ManagerGroup.Instance.GetManager<JHT_RoomManager>().SetGameCustomProperty(false);
        StateCustomProperty(CurrentState.InRoom);

        yield return new WaitUntil(() =>
       PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("CurState") &&
       (CurrentState)PhotonNetwork.LocalPlayer.CustomProperties["CurState"] == CurrentState.InRoom);

        OnRoomIn?.Invoke(true, true);
        Debug.Log($"현재 상태 {PhotonNetwork.LocalPlayer.CustomProperties["CurState"]}");
        Debug.Log($"{PhotonNetwork.LocalPlayer.ActorNumber} 생성 액터넘버");
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
            ManagerGroup.Instance.GetManager<JHT_TeamManager>().SetPlayerTeam(newPlayer);

        while (!newPlayer.CustomProperties.ContainsKey("Team"))
            yield return null;

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer != PhotonNetwork.LocalPlayer)
        {
            ManagerGroup.Instance.GetManager<JHT_RoomManager>().PlayerLeaveRoom(otherPlayer);
        }
    }
    public override void OnMasterClientSwitched(Player newClientPlayer)
    {
        ManagerGroup.Instance.GetManager<JHT_RoomManager>().PlayerPanelSpawn(newClientPlayer);
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

                prefab.transform.SetParent(OnParent?.Invoke());
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
        CurrentState curstate = _curPlayerState;
        curState["CurState"] = curstate; // 위에까지 작성과정
        PhotonNetwork.LocalPlayer.SetCustomProperties(curState); //전역변수 저장 느낌
    }

    public void SetHeroCustomProperty(TeamSetting setting)
    {
        int setRedCount = 0;
        int setBlueCount = 0;
        ExitGames.Client.Photon.Hashtable roles = new();
        int setTeam = (int)setting;
        string setJob = null;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team",out object value))
        {
            if ((int)value == 0)
            {
                setJob = setRedCount == 0 ? "Hero" : "Command";
                setRedCount++;
            }
            else
            {
                setJob = setBlueCount == 0 ? "Hero" : "Command";
                setBlueCount++;
            }
        }
        else
        {
            Debug.LogError("NetworkManager - ConnectGameScene] 게임 시작시 팀정보 없음");
        }

        roles["Team"] = setTeam;
        roles["Role"] = setJob;
        PhotonNetwork.LocalPlayer.SetCustomProperties(roles);
    }

    // 프로퍼티 변화가 생겼을때 호출, 호출이 되어야할 로직이 있을 사용
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("RedCount"))
            ManagerGroup.Instance.GetManager<JHT_TeamManager>().redCount = (int)propertiesThatChanged["RedCount"];
        if (propertiesThatChanged.ContainsKey("BlueCount"))
            ManagerGroup.Instance.GetManager<JHT_TeamManager>().blueCount = (int)propertiesThatChanged["BlueCount"];

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Team"))
        {
            ManagerGroup.Instance.GetManager<JHT_RoomManager>().OtherPlayerChangeTeam(targetPlayer);
        }

        if(changedProps.ContainsKey("IsReady"))
        {
            StartCoroutine(WaitForAddDic(targetPlayer, changedProps));
        }

        if(changedProps.ContainsKey("Character"))
        {
            StartCoroutine(WaitForLoadCharacter(targetPlayer, changedProps));
        }

        if (changedProps.ContainsKey("GamePlay"))
        {
            StartCoroutine(GameStartCor(targetPlayer, changedProps));
        }

    }

    IEnumerator GameStartCor(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        yield return null;
        if (changedProps.TryGetValue("GamePlay",out object value))
        {
            if(mainLobbyPanel == null)
                mainLobbyPanel = FindObjectOfType<JHT_NetworkUIPanel>();

            if ((bool)value)
            {
                if(mainLobbyPanel.gameObject.activeSelf)
                    mainLobbyPanel.gameObject.SetActive(!(bool)value);

                ConnectGameScene();
            }
            else
            {
                if (mainLobbyPanel.gameObject.activeSelf)
                    mainLobbyPanel.gameObject.SetActive(!(bool)value);
            }
        }
    }

    IEnumerator WaitForAddDic(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        while (!ManagerGroup.Instance.GetManager<JHT_RoomManager>().playerPanelDic.ContainsKey(targetPlayer.ActorNumber))
            yield return null;


        if (changedProps.ContainsKey("IsReady"))
        {
            if (ManagerGroup.Instance.GetManager<JHT_RoomManager>().playerPanelDic.TryGetValue(targetPlayer.ActorNumber, out var panel))
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
        while (!ManagerGroup.Instance.GetManager<JHT_RoomManager>().playerPanelDic.ContainsKey(targetPlayer.ActorNumber))
            yield return null;

        if (changedProps.ContainsKey("Character"))
        {
            if (ManagerGroup.Instance.GetManager<JHT_RoomManager>().playerPanelDic.TryGetValue(targetPlayer.ActorNumber, out var panel))
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

    #region Method

    public void ConnectGameScene()
    {
        StartCoroutine(WaitForLoad(PhotonNetwork.LocalPlayer));
    }

    IEnumerator WaitForLoad(Player player)
    {
        //1. FadeIn

        //2. 씬전환시작
        PhotonNetwork.LoadLevel("GameScenes");
        

        //3. 로딩후 씬 준비

        //4. 씬 완료시 while문 시작
        while (!player.CustomProperties.ContainsKey("Team") || !player.CustomProperties.ContainsKey("Character") ||
            !player.CustomProperties.ContainsKey("Role"))
            yield return null;

        int playerIndex = (int)player.CustomProperties["Character"];

        SetHeroCustomProperty((TeamSetting)player.CustomProperties["Team"]);
        //5. ManagerGroup.Instance.GetManager<KMS_InGameNetWorkManager>().SetRole(playerIndex);

        //6. FadeOut
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log($"현재 방 인원 수 : {PhotonNetwork.CurrentRoom.PlayerCount}");
        }
    }
}
