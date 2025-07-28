using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;


//public enum TestTeamSetting
//{
//    Red,
//    Blue
//}

public class LGH_TestNetwork : MonoBehaviourPunCallbacks
{

    public Transform redSpawnPoint;
    public Transform blueSpawnPoint;

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
        // �� ������Ƽ�� ������ ������ ���
        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RedCount"))
            yield return null;

        AssignTeam();
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
        Vector3 spawnPos = testTeam == TestTeamSetting.Red ? redSpawnPoint.position : blueSpawnPoint.position;
        Quaternion spawnRot = testTeam == TestTeamSetting.Red ? redSpawnPoint.rotation : blueSpawnPoint.rotation;

        // ����
        PhotonNetwork.Instantiate("Hero1", spawnPos, spawnRot);
        Debug.Log($"JHT_TestNetwork : �÷��̾� {PhotonNetwork.NickName} �� : {testTeam}, ��ġ : {spawnPos}");
    }
}
