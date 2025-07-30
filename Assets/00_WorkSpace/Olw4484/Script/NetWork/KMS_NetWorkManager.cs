using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;


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

    // 디버깅 모드
#if UNITY_EDITOR
    public bool isHeroDebugMode = false;
    public bool isCommandDebugMode = false;
#endif
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
        if (isCommandDebugMode)
        {
            PhotonNetwork.Instantiate("HQ", cmdRedSpawnPoint.position, cmdRedSpawnPoint.rotation);
        }
        if (isHeroDebugMode)
        {
            PhotonNetwork.Instantiate("Hero1", heroRedSpawnPoint.position, heroRedSpawnPoint.rotation);
        }

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 4)
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
            // Hero만 생성
            Vector3 pos = (myTeamId == 0) ? heroRedSpawnPoint.position : heroBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? heroRedSpawnPoint.rotation : heroBlueSpawnPoint.rotation;
            PhotonNetwork.Instantiate("Hero1", pos, rot);
        }
        else if (myRole == "Command")
        {
            // 1. HQ 먼저 생성 (내가 그 팀의 Command라면)
            Vector3 hqPos = (myTeamId == 0) ? hqRedSpawnPoint.position : hqBlueSpawnPoint.position;
            Quaternion hqRot = (myTeamId == 0) ? hqRedSpawnPoint.rotation : hqBlueSpawnPoint.rotation;
            PhotonNetwork.Instantiate("HQ", hqPos, hqRot);

            // 2. CommandPlayer 생성 및 Canvas 연결
            Vector3 pos = (myTeamId == 0) ? cmdRedSpawnPoint.position : cmdBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? cmdRedSpawnPoint.rotation : cmdBlueSpawnPoint.rotation;
            var cmdObj = PhotonNetwork.Instantiate("CommandPlayer", pos, rot);

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
    #region DebugMode
    private void DebugHeroSpawn()
    {
        Vector3 pos = heroRedSpawnPoint.position;
        Quaternion rot = heroRedSpawnPoint.rotation;

        Instantiate(Resources.Load<GameObject>("Hero1"), pos, rot);
    }

    private void DebugHQspwn()
    {
        // HQ를 네트워크로 생성
        Vector3 hqPos = hqRedSpawnPoint.position;
        Quaternion hqRot = hqRedSpawnPoint.rotation;
        PhotonNetwork.Instantiate("HQ", hqPos, hqRot);

        // CommandPlayer도 네트워크로 생성
        Vector3 pos = cmdRedSpawnPoint.position;
        Quaternion rot = cmdRedSpawnPoint.rotation;
        var go = PhotonNetwork.Instantiate("CommandPlayer", pos, rot);

        // Canvas 세팅 (내꺼만)
        if (go.GetComponent<PhotonView>().IsMine)
        {
            var commandPlayer = go.GetComponent<CommandPlayer>();
            var canvasObj = Instantiate(canvasPrefab);
            commandPlayer.goldText = canvasObj.transform.Find("ResourcePanel/GoldText").GetComponent<TMP_Text>();
            commandPlayer.gearText = canvasObj.transform.Find("ResourcePanel/GearText").GetComponent<TMP_Text>();
            commandPlayer.playerInputHandler = canvasObj.GetComponent<PlayerInputHandler>();
        }
    }
    #endregion
}