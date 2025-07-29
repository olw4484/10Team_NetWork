using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
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

    #region 플레이어 패널 생성
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
            }
            else
            {
                Debug.Log($"플레이어 {player.NickName}에 대한 정보 없음");
            }
        }
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
    #endregion


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


    //플레이어가 방을 떠났을 때
    public void PlayerLeaveRoom(Player player)
    {
        if (playerPanelDic.TryGetValue(player.ActorNumber, out JHT_PlayerPanelItem obj))
        {
            playerPanelDic.Remove(player.ActorNumber);
            Destroy(obj.gameObject);
        }

    }

    public void GameStart()
    {
        
    }

    //내 플레이어가 방을 떠났을 때
    public void LeaveRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Destroy(playerPanelDic[player.ActorNumber].gameObject);
        }
        playerPanelDic.Clear();
        PhotonNetwork.LeaveRoom();
    }

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
