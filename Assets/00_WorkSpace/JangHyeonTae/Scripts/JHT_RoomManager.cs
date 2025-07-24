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

    private bool isRed;
    public List<GameObject> redTeam = new List<GameObject>(2);
    public List<GameObject> blueTeam =  new List<GameObject>(2);

    private Dictionary<int, JHT_PlayerPanelItem> playerPanelDic = new();

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

        GameObject obj = Instantiate(playerPanelPrefab);

        if (SetTeam(redTeam,blueTeam, out int num))
        {
            obj.transform.SetParent(playerRedPanelParent);
        }
        else
        {
            obj.transform.SetParent(playerBluePanelParent);
        }

        JHT_PlayerPanelItem playerPanel = obj.GetComponent<JHT_PlayerPanelItem>();
        playerPanel.Init(player);
        playerPanelDic.Add(player.ActorNumber, playerPanel);
    }

    public void PlayerPanelSpawn()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject obj = Instantiate(playerPanelPrefab);

            if(SetTeam(redTeam, blueTeam, out int num))
            {
                obj.transform.SetParent(playerRedPanelParent);
                redTeam[num] = obj;
            }
            else
            {
                obj.transform.SetParent(playerBluePanelParent);
                blueTeam[num] = obj;
            }

            JHT_PlayerPanelItem playerPanel = obj.GetComponent<JHT_PlayerPanelItem>();
            playerPanel.Init(player); 
            playerPanelDic.Add(player.ActorNumber, playerPanel);
        }
    }

    public void PlayerLeaveRoom(Player player)
    {
        if (playerPanelDic.TryGetValue(player.ActorNumber, out JHT_PlayerPanelItem obj))
        {
            playerPanelDic.Remove(player.ActorNumber);
            Destroy(obj.gameObject);
        }
    }

    public bool SetTeam(List<GameObject> firstArr,List<GameObject> secondArr,out int idx)
    {
        bool isMySelect = false;
        for (int i = 0; i < firstArr.Count; i++)
        {
            if (firstArr[i] == null)
            {
                isMySelect = true;
                idx = i;
                return true;
            }
        }

        for (int i = 0; i < secondArr.Count; i++)
        {
            if (secondArr[i] == null)
            {
                isMySelect = false;
                idx = i;
                return true;
            }
        }

        idx = -1;
        return isMySelect;
    }
    public void PlayerTeamChange(Player player, Func<List<GameObject>, List<GameObject>, int,bool> action)
    {
        
    }

    public void GameStart()
    {

    }
    public void LeaveRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Destroy(playerPanelDic[player.ActorNumber].gameObject);
        }

        playerPanelDic.Clear();

        PhotonNetwork.LeaveRoom();

    }

}
