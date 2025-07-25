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

    private bool isRed;
    public Dictionary<int, JHT_PlayerPanelItem> playerPanelDic = new();
    public Dictionary<int, string> teamDic = new();

    private int blueCount;
    private int redCount;


    private void Start()
    {
        startButton.onClick.AddListener(GameStart);
        leaveRoomButton.onClick.AddListener(LeaveRoom);
    }

    public void PlayerPanelSpawn(Player player)
    {
        if (playerPanelDic.TryGetValue(player.ActorNumber, out JHT_PlayerPanelItem panel))
        {
            startButton.interactable = true;
            panel.Init(player);
            return;
        }

        GameObject inst = InstAndSetParent(player);

        JHT_PlayerPanelItem playerPanel = inst.GetComponent<JHT_PlayerPanelItem>();
        playerPanel.Init(player);
        playerPanelDic.Add(player.ActorNumber, playerPanel);
        teamDic.Add(player.ActorNumber, (string)player.CustomProperties["Team"]);
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
            GameObject inst = InstAndSetParent(player);

            JHT_PlayerPanelItem playerPanel = inst.GetComponent<JHT_PlayerPanelItem>();
            playerPanel.Init(player);

            playerPanelDic.Add(player.ActorNumber, playerPanel);
            teamDic.Add(player.ActorNumber, (string)player.CustomProperties["Team"]);
        }
    }

    //팀 : 부모정하기
    public GameObject InstAndSetParent(Player player)
    {
        GameObject obj = Instantiate(playerPanelPrefab);

        if (player.CustomProperties.TryGetValue("Team", out object team))
        {
            if ((string)team == "Blue")
                obj.transform.SetParent(playerBluePanelParent);
            else
                obj.transform.SetParent(playerRedPanelParent);

            return obj;
        }
        else
        {
            Debug.Log($"플레이어 {player.NickName} 팀 정보 없음");
            return null;
        }
    }

    #region 팀 : 팀나누기
    public void SeparateTeamCustomProperty(Player player)
    {
        string team = blueCount >= redCount ? "Red" : "Blue";

        ExitGames.Client.Photon.Hashtable teamProp = new();
        teamProp["Team"] = team;
        PhotonNetwork.LocalPlayer.SetCustomProperties(teamProp);

        if (team == "Blue")
            blueCount++;
        else 
            redCount++;
    }

    public void ResetTeamCount()
    {
        blueCount = 0;
        redCount = 0;
    }
    #endregion



    //플레이어가 방을 떠났을 때
    public void PlayerLeaveRoom(Player player)
    {
        if (playerPanelDic.TryGetValue(player.ActorNumber, out JHT_PlayerPanelItem obj))
        {
            playerPanelDic.Remove(player.ActorNumber);
            Destroy(obj.gameObject);
        }

        if (teamDic.ContainsKey(player.ActorNumber))
        {
            if ((string)player.CustomProperties["Team"] == "Red")
                redCount--;
            else
                blueCount--;

            teamDic.Remove(player.ActorNumber);
        }
    }

    public void GameStart()
    {
        if (teamDic.Count == 4)
        {

        }
    }

    //내 플레이어가 방을 떠났을 때
    public void LeaveRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Destroy(playerPanelDic[player.ActorNumber].gameObject);
        }
        playerPanelDic.Clear();
        teamDic.Clear();
        PhotonNetwork.LeaveRoom();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} : {PhotonNetwork.LocalPlayer.CustomProperties["Team"]}");
        }
    }
}
