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

    private int blueCount;
    private int redCount;


    private void Start()
    {
        startButton.onClick.AddListener(GameStart);
        leaveRoomButton.onClick.AddListener(LeaveRoom);
    }

    public void PlayerPanelSpawn(Player player,Transform parent)
    {
        if (playerPanelDic.TryGetValue(player.ActorNumber, out JHT_PlayerPanelItem panel))
        {
            startButton.interactable = true;
            panel.Init(player);
            return;
        }

        GameObject obj = Instantiate(playerPanelPrefab);
        obj.transform.SetParent(parent);
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
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Team", out object team))
            {
                if (team == null)
                {
                    Debug.Log($"PlayerPanelSpawn : team null");
                }

                TeamSetting setting = (TeamSetting)team;
                Transform _parent = setting == TeamSetting.Red ? playerRedPanelParent : playerBluePanelParent;

                if (_parent == null)
                {
                    Debug.Log($"PlayerPanelSpawn : {_parent} null");
                }

                // 오브젝트 생성 및 지정한 부모에 넣기
                GameObject obj = Instantiate(playerPanelPrefab);
                obj.transform.SetParent(_parent);
                if (obj == null)
                {
                    Debug.Log($"PlayerPanelSpawn : {obj} null");
                }
                
                JHT_PlayerPanelItem playerPanel = obj.GetComponent<JHT_PlayerPanelItem>();
                playerPanel.Init(player);

                if (playerPanel == null)
                {
                    Debug.Log($"PlayerPanelSpawn : {playerPanel} null");
                }
                
                //딕셔너리에 넣기
                playerPanelDic.Add(player.ActorNumber, playerPanel);
            }
            else
            {
                Debug.Log($"플레이어 {player.NickName}에 대한 정보 없음");
            }
            Debug.Log($"RoomCount : {PhotonNetwork.PlayerList.Length}");
        }
    }


    #region 팀 : 팀나누기
    //public void SeparateTeamCustomProperty(Player player)
    //{
    //    blueCount = 0;
    //    redCount = 0;
    //    string team = blueCount >= redCount ? "Red" : "Blue";
    //
    //    ExitGames.Client.Photon.Hashtable teamProp = new();
    //    teamProp["Team"] = team;
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(teamProp);
    //
    //    if (team == "Blue")
    //        blueCount++;
    //    else 
    //        redCount++;
    //}

    #endregion



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
                Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} : {teamObj.ToString()}");
            }
            else
            {
                Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}의 팀 정보 없음.");
            }
        }
    }
}
