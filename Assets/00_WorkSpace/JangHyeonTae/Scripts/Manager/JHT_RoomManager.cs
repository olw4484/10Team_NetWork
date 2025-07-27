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

    //����
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

                // ������Ʈ ���� �� ������ �θ� �ֱ�
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
                
                //��ųʸ��� �ֱ�
                playerPanelDic.Add(player.ActorNumber, playerPanel);
            }
            else
            {
                Debug.Log($"�÷��̾� {player.NickName}�� ���� ���� ����");
            }
            Debug.Log($"RoomCount : {PhotonNetwork.PlayerList.Length}");
        }
    }


    #region �� : ��������
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



    //�÷��̾ ���� ������ ��
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

    //�� �÷��̾ ���� ������ ��
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
                Debug.Log($"{PhotonNetwork.LocalPlayer.NickName}�� �� ���� ����.");
            }
        }
    }
}
