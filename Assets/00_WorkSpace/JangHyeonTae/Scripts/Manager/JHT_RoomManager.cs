using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JHT_RoomManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPanelPrefab;
    [SerializeField] private Transform playerRedPanelParent;
    [SerializeField] private Transform playerBluePanelParent;
    [SerializeField] private Button startButton;
    [SerializeField] private Button leaveRoomButton;

    //보류
    [SerializeField] private Button redButton;
    [SerializeField] private Button blueButton;

    public Dictionary<int, JHT_PlayerPanelItem> playerPanelDic = new();
    [SerializeField] private JHT_TeamManager teamManager;

    private void Start()
    {
        startButton.onClick.AddListener(GameStart);
        leaveRoomButton.onClick.AddListener(LeaveRoom);
        teamManager.OnChangeTeam += ChangeTeam;
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(GameStart);
        leaveRoomButton.onClick.RemoveListener(LeaveRoom);
        teamManager.OnChangeTeam -= ChangeTeam;
    }

    #region 원래는 다른플레이어 패널 생성 이었지만 지금은 마스터 클라이언트가 바뀔시에만 사용(OnPlayerPropertiesUpdate에서 다른 플레이어 생성)
    public void PlayerPanelSpawn(Player player)
    {
        if (playerPanelDic.TryGetValue(player.ActorNumber, out JHT_PlayerPanelItem panel))
        {
            startButton.interactable = true;
            panel.Init(player);
            return;
        }

        GameObject obj = Instantiate(playerPanelPrefab);
        obj.transform.SetParent(SetPanelParent(player));
        JHT_PlayerPanelItem playerPanel = obj.GetComponent<JHT_PlayerPanelItem>();
        playerPanel.Init(player);
        playerPanelDic.Add(player.ActorNumber, playerPanel);
    }
    #endregion

    #region 플레이어 패널 생성
    public void PlayerPanelSpawn()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsMasterClient)
        {
            startButton.interactable = false;
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("Team", out object team))
            {
                GameObject obj = Instantiate(playerPanelPrefab);
                obj.transform.SetParent(SetPanelParent(player));
                JHT_PlayerPanelItem playerPanel = obj.GetComponent<JHT_PlayerPanelItem>();
                playerPanel.Init(player);
                playerPanelDic.Add(player.ActorNumber, playerPanel);
                Debug.Log($"My Player 딕셔너리에 추가  : Key - {player.ActorNumber}, Value - {playerPanel}");
            }
            else
            {
                Debug.Log($"플레이어 {player.NickName}에 대한 정보 없음");
            }
        }


        Debug.Log($"RedCount : {PhotonNetwork.CurrentRoom.CustomProperties["RedCount"].ToString()}");
        Debug.Log($"BlueCount : {PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"].ToString()}");
    }
    #endregion


    #region 팀바꾸기
    public void ChangeTeam(Player player, int red, int blue)
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (playerPanelDic.TryGetValue(player.ActorNumber, out var panel))
        {
            Destroy(panel.gameObject);
            playerPanelDic.Remove(player.ActorNumber);
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            startButton.interactable = false;
        }

        StartCoroutine(SetTeamCor(player,red,blue));

    }

    private IEnumerator SetTeamCor(Player player,int _red,int _blue)
    {

        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RedCount") ||
           !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BlueCount"))
        {
            yield return null;
        }

        teamManager.SetChangePlayerTeam(player, _red, _blue);

        while (!player.CustomProperties.ContainsKey("Team"))
            yield return null;

        yield return new WaitForSeconds(0.2f);
        GameObject obj = Instantiate(playerPanelPrefab);
        obj.transform.SetParent(SetPanelParent(player));
        JHT_PlayerPanelItem playerPanel = obj.GetComponent<JHT_PlayerPanelItem>();
        playerPanel.Init(player);
        playerPanelDic.Add(player.ActorNumber, playerPanel);
    }


    //팀바꾸기 동기화(OnPlayerPropertiesUpdate에서 생성) -> 각 플레이어가 역할을 팀을 배정받을 때 생성
    // -> PlayerPanelSpawn(Player player)의 역할 대체
    public void OtherPlayerChangeTeam(Player player)
    {
        if (player == PhotonNetwork.LocalPlayer)
            return;

        if (playerPanelDic.TryGetValue(player.ActorNumber, out var panel))
        {
            Destroy(panel.gameObject);
            playerPanelDic.Remove(player.ActorNumber);
        }

        StartCoroutine(OtherPlayerSetTeamCor(player));
    }
    private IEnumerator OtherPlayerSetTeamCor(Player player)
    {

        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RedCount") ||
           !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BlueCount"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f); 
        

        GameObject obj = Instantiate(playerPanelPrefab);
        obj.transform.SetParent(SetPanelParent(player));
        JHT_PlayerPanelItem newPanel = obj.GetComponent<JHT_PlayerPanelItem>();
        newPanel.Init(player);
        playerPanelDic.Add(player.ActorNumber, newPanel);


    }
    #endregion

    #region 모든 오브젝트의 부모 설정 -> CustomProperty에서 받아서 사용
    public Transform SetPanelParent(Player player)
    {
        if (player.CustomProperties.TryGetValue("Team", out object value))
        {
            if ((int)value == (int)TeamSetting.Blue)
            {
                return playerBluePanelParent;
            }
            else
            {
                return playerRedPanelParent;
            }
        }
        else
        {
            Debug.Log($"TeamManager SetParentFromCustomProperty 팀 정보 없음 {PhotonNetwork.LocalPlayer.ActorNumber}");
            return null;
        }
    }
    #endregion

    #region 다른 플레이어가 떠났을경우
    public void PlayerLeaveRoom(Player player)
    {
        if (playerPanelDic.TryGetValue(player.ActorNumber, out JHT_PlayerPanelItem obj))
        {
            playerPanelDic.Remove(player.ActorNumber);
            Destroy(obj.gameObject);
        }

        ExitGames.Client.Photon.Hashtable props = new();

        if ((TeamSetting)player.CustomProperties["Team"] == TeamSetting.Blue)
        {
            int currentBlue = (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"];
            if (currentBlue > 0)
            {
                props["BlueCount"] = currentBlue - 1;
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                Debug.Log("Leave blueteam");
            }
        }
        else if ((TeamSetting)player.CustomProperties["Team"] == TeamSetting.Red)
        {
            int currentRed = (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"];
            if (currentRed > 0)
            {
                props["RedCount"] = currentRed - 1;
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                Debug.Log("Leave redteam");
            }
        }


        Debug.Log($"RedCount : {PhotonNetwork.CurrentRoom.CustomProperties["RedCount"].ToString()}");
        Debug.Log($"BlueCount : {PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"].ToString()}");
    }
    #endregion

    #region 게임시작
    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient && AdllPlayerReadyCheck()
            && (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"] == 2
            && (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"] == 2)
        {
            PhotonNetwork.LoadLevel("GameScene"); //해당 게임씬 넣기 
        }
    }

    public bool AdllPlayerReadyCheck()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.TryGetValue("IsReady", out object value)||(bool)value)
            {
                return false;
            }
        }

        return true;
    }
    #endregion

    #region 내가 방을 나갔을경우
    public void LeaveRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (playerPanelDic.ContainsKey(player.ActorNumber))
                Destroy(playerPanelDic[player.ActorNumber].gameObject);
        }
        playerPanelDic.Clear();

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object value))
        {
            if ((TeamSetting)value == TeamSetting.Blue || (TeamSetting)value == TeamSetting.Red)
            {
                ExitGames.Client.Photon.Hashtable props = new();
                props["Team"] = null;
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }
        }

        PhotonNetwork.LeaveRoom();
    }
    #endregion

    //스페이스바 누르면 해당 플레이어 - 팀 정보 나옴
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object teamObj) && teamObj != null)
            {
                Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} 번 팀원 , 팀 : {teamObj.ToString()}");
            }
            else
            {
                Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}의 팀 정보 없음.");
            }
        }
    }
}
