using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//public enum TestTeamSetting
//{
//    Red,
//    Blue
//}

public class KMS_TestNetwork : MonoBehaviourPunCallbacks
{

    public Transform heroRedSpawnPoint;
    public Transform heroBlueSpawnPoint;

    public Transform hqRedSpawnPoint;
    public Transform hqBlueSpawnPoint;
    public Transform cmdRedSpawnPoint;
    public Transform cmdBlueSpawnPoint;

    public GameObject canvasPrefab;

    private void Start()
    {
        string randomName = $"Tester{UnityEngine.Random.Range(1000, 9999)}";
        ConnectToPhoton(randomName);
    }

    private void ConnectToPhoton(string nickName)
    {
        Debug.Log($"Connect to Photon as {nickName}");
        PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"{PhotonNetwork.NickName} JoinRoom");
        // MasterClient�� �� �ο� ���� �ʱ�ȭ
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable init = new();
            init["RedCount"] = 0;
            init["BlueCount"] = 0;
            PhotonNetwork.CurrentRoom.SetCustomProperties(init);
        }

        StartCoroutine(WaitForRoomPropertiesAndJoin());
    }

    private IEnumerator WaitForRoomPropertiesAndJoin()
    {
        AssignTeam();

        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            yield return null;

        int myTeamId = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];

        // HQ/CommandPlayer ����ó�� ����
        Vector3 hqSpawnPos, cmdSpawnPos, heroSpawnPos;
        Quaternion hqSpawnRot, cmdSpawnRot, heroSpawnRot;

        if (myTeamId == 0) // Red
        {
            hqSpawnPos = hqRedSpawnPoint.position;
            hqSpawnRot = hqRedSpawnPoint.rotation;
            cmdSpawnPos = cmdRedSpawnPoint.position;
            cmdSpawnRot = cmdRedSpawnPoint.rotation;
            heroSpawnPos = heroRedSpawnPoint.position;
            heroSpawnRot = heroRedSpawnPoint.rotation;
        }
        else // Blue
        {
            hqSpawnPos = hqBlueSpawnPoint.position;
            hqSpawnRot = hqBlueSpawnPoint.rotation;
            cmdSpawnPos = cmdBlueSpawnPoint.position;
            cmdSpawnRot = cmdBlueSpawnPoint.rotation;
            heroSpawnPos = heroBlueSpawnPoint.position;
            heroSpawnRot = heroBlueSpawnPoint.rotation;
        }

        // ���� HQ, CommandPlayer ����
        var hqObj = PhotonNetwork.Instantiate("HQ", hqSpawnPos, hqSpawnRot);
        hqObj.GetComponent<HQ>().teamId = myTeamId;

        var cmdObj = PhotonNetwork.Instantiate("CommandPlayer", cmdSpawnPos, cmdSpawnRot);
        cmdObj.GetComponent<CommandPlayer>().teamId = myTeamId;

        var commander = hqObj.GetComponent<HQCommander>();
        if (commander != null)
            commander.player = cmdObj.GetComponent<CommandPlayer>();

        // Hero ���� (���⸸ �߰�)
        var heroObj = PhotonNetwork.Instantiate("Hero1", heroSpawnPos, heroSpawnRot);

        // �� �÷��̾ Canvas ����
        var commandPlayer = cmdObj.GetComponent<CommandPlayer>();
        if (commandPlayer.photonView.IsMine)
        {
            var canvasObj = Instantiate(canvasPrefab);
            commandPlayer.goldText = canvasObj.transform.Find("ResourcePanel/GoldText").GetComponent<TMP_Text>();
            commandPlayer.gearText = canvasObj.transform.Find("ResourcePanel/GearText").GetComponent<TMP_Text>();
            commandPlayer.playerInputHandler = canvasObj.GetComponent<PlayerInputHandler>();
        }
    }

    private void AssignTeam()
    {
        int redCount = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RedCount") ?
                       (int)PhotonNetwork.CurrentRoom.CustomProperties["RedCount"] : 0;
        int blueCount = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BlueCount") ?
                        (int)PhotonNetwork.CurrentRoom.CustomProperties["BlueCount"] : 0;

        TestTeamSetting testTeam;
        string countKey;

        if (blueCount < redCount)
        {
            testTeam = TestTeamSetting.Blue;
            blueCount++;
            countKey = "BlueCount";
        }
        else
        {
            testTeam = TestTeamSetting.Red;
            redCount++;
            countKey = "RedCount";
        }

        // �� �� Custom Property ����
        ExitGames.Client.Photon.Hashtable teamProp = new();
        teamProp["Team"] = testTeam;
        PhotonNetwork.LocalPlayer.SetCustomProperties(teamProp);

        // �濡 �ݿ�
        ExitGames.Client.Photon.Hashtable roomProps = new();
        roomProps[countKey] = testTeam == TestTeamSetting.Red ? redCount : blueCount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);   

        //���� ���� ���� ��ġ ����
        Vector3 spawnPos = testTeam == TestTeamSetting.Red ? heroRedSpawnPoint.position : heroBlueSpawnPoint.position;
        Quaternion spawnRot = testTeam == TestTeamSetting.Red ? heroRedSpawnPoint.rotation : heroBlueSpawnPoint.rotation;

    }
}
