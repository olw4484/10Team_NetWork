using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;


//public enum TestTeamSetting
//{
//    Red,
//    Blue
//}

public class LGH_TestNetwork : MonoBehaviourPunCallbacks
{

    public Transform redSpawnPoint;
    public Transform blueSpawnPoint;

    [Header("테스트 씬 상세")]
    [SerializeField] private const string testLobbyName = "TestLobby";
    [SerializeField] private const string testGameVersion = "Test_1.0";

    private void Start()
    {
        string randomName = $"Tester{UnityEngine.Random.Range(1000, 9999)}";
        ConnectToPhoton(randomName);
    }

    private void ConnectToPhoton(string nickName)
    {
        Debug.Log($"Connect to Photon as {nickName}");
        PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
        PhotonNetwork.AutomaticallySyncScene = true;   // 테스트씬에서는 씬동기화 꺼둠
        PhotonNetwork.NickName = nickName;
        PhotonNetwork.GameVersion = testGameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        TypedLobby testLobby = new TypedLobby(testLobbyName, LobbyType.Default);
        PhotonNetwork.JoinLobby(testLobby);
    }

    public override void OnJoinedLobby()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = false; // 테스트 방은 숨김처리
        roomOptions.IsOpen = true;

        TypedLobby testLobby = new TypedLobby(testLobbyName, LobbyType.Default);
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"{PhotonNetwork.NickName} JoinRoom");
        // MasterClient가 팀 인원 수를 초기화
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable init = new();
            init["RedCount"] = 0;
            init["BlueCount"] = 0;
            PhotonNetwork.CurrentRoom.SetCustomProperties(init);
        }
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("ItemSpawner", Vector3.zero, Quaternion.identity);
        StartCoroutine(WaitForRoomPropertiesAndJoin());
    }

    private IEnumerator WaitForRoomPropertiesAndJoin()
    {
        // 방 프로퍼티가 설정될 때까지 대기
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

        // 내 팀 Custom Property 설정
        ExitGames.Client.Photon.Hashtable teamProp = new();
        teamProp["Team"] = testTeam;
        PhotonNetwork.LocalPlayer.SetCustomProperties(teamProp);

        // 방에 반영
        ExitGames.Client.Photon.Hashtable roomProps = new();
        roomProps[countKey] = testTeam == TestTeamSetting.Red ? redCount : blueCount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

        //팀에 따라 스폰 위치 결정
        Vector3 spawnPos = testTeam == TestTeamSetting.Red ? redSpawnPoint.position : blueSpawnPoint.position;
        Quaternion spawnRot = testTeam == TestTeamSetting.Red ? redSpawnPoint.rotation : blueSpawnPoint.rotation;

        // 생성
        PhotonNetwork.Instantiate("Hero0", spawnPos, spawnRot);

        Debug.Log($"[TEST] 플레이어 {PhotonNetwork.NickName}의 팀 : {testTeam}, 위치 : {spawnPos}");
    }
}
