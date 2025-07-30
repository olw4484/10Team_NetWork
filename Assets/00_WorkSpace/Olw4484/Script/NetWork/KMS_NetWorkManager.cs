using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;


//public enum TestTeamSetting
//{
//    Red,
//    Blue
//}

public class KMS_NetWorkManager : MonoBehaviourPunCallbacks
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

    // 방에 들어왔을 때 팀/역할 배정 (MasterClient만 처리 - 동시 접속으로 인한 중복방지)
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int red = 0, blue = 0;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                // Team - 0: Red, 1: Blue
                // Role - 0: Hero, 1: Command
                ExitGames.Client.Photon.Hashtable props = new();
                if (red <= blue)
                {
                    props["Team"] = 0;
                    props["Role"] = red == 0 ? "Hero" : "Command";
                    red++;
                }
                else
                {
                    props["Team"] = 1;
                    props["Role"] = blue == 0 ? "Hero" : "Command";
                    blue++;
                }
                player.SetCustomProperties(props);
            }
        }
        // 배정이 완료될 때까지 대기 후 스폰 진행
        StartCoroutine(WaitForRoomPropertiesAndJoin());
    }

    private IEnumerator WaitForRoomPropertiesAndJoin()
    {
        // Role이 할당될 때까지 대기
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Role"))
            yield return null;
        // Team이 할당될 때까지 대기
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            yield return null;

        // 배정된 정보로 내 플레이어 오브젝트 스폰
        SetRole();
    }

    // 역할에 따라 오브젝트 스폰
    private void SetRole()
    {
        int myTeamId = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        string myRole = (string)PhotonNetwork.LocalPlayer.CustomProperties["Role"];

        if (myRole == "Hero")
        {
            // 팀에 따라 Hero 스폰
            Vector3 pos = (myTeamId == 0) ? heroRedSpawnPoint.position : heroBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? heroRedSpawnPoint.rotation : heroBlueSpawnPoint.rotation;
            PhotonNetwork.Instantiate("Hero1", pos, rot);
        }
        else if (myRole == "Command")
        {
            // 팀에 따라 CommandPlayer 스폰 및 Canvas 세팅
            Vector3 pos = (myTeamId == 0) ? cmdRedSpawnPoint.position : cmdBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? cmdRedSpawnPoint.rotation : cmdBlueSpawnPoint.rotation;
            var cmdObj = PhotonNetwork.Instantiate("CommandPlayer", pos, rot);

            // 내 플레이어(로컬)만 Canvas 인스턴스 생성 및 연결
            if (cmdObj.GetComponent<PhotonView>().IsMine)
            {
                var canvasObj = Instantiate(canvasPrefab);
                var commandPlayer = cmdObj.GetComponent<CommandPlayer>();
                commandPlayer.goldText = canvasObj.transform.Find("ResourcePanel/GoldText").GetComponent<TMP_Text>();
                commandPlayer.gearText = canvasObj.transform.Find("ResourcePanel/GearText").GetComponent<TMP_Text>();
                commandPlayer.playerInputHandler = canvasObj.GetComponent<PlayerInputHandler>();
            }
        }
    }
}
