using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    // ����� ���
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

    // �濡 ������ �� ��/���� ���� (MasterClient�� ó�� - ���� �������� ���� �ߺ�����)
    public override void OnJoinedRoom()
    {
        if (isCommandDebugMode)
        {
            // HQ, CommandPlayer�� ���� ��ġ�� ����
            var hqObj = PhotonNetwork.Instantiate("HQ", cmdRedSpawnPoint.position, cmdRedSpawnPoint.rotation);
            var cmdObj = PhotonNetwork.Instantiate("CommandPlayer", cmdRedSpawnPoint.position, cmdRedSpawnPoint.rotation);

            // HQ - CommandPlayer ����
            var commandPlayer = cmdObj.GetComponent<CommandPlayer>();
            var hq = hqObj.GetComponent<HQCommander>();
            hq.player = commandPlayer;

            // Canvas �� ��ǲ �� ����
            if (commandPlayer.photonView.IsMine)
            {
                var canvasObj = Instantiate(canvasPrefab);
                commandPlayer.goldText = canvasObj.transform.Find("ResourcePanel/GoldText").GetComponent<TMP_Text>();
                commandPlayer.gearText = canvasObj.transform.Find("ResourcePanel/GearText").GetComponent<TMP_Text>();
                commandPlayer.playerInputHandler = canvasObj.GetComponent<PlayerInputHandler>();

                // --- ��ư �̺�Ʈ ���� ---
                var meleeBtn = canvasObj.transform.Find("MinionPanel/MeleeButton").GetComponent<Button>();
                var rangedBtn = canvasObj.transform.Find("MinionPanel/RangedButton").GetComponent<Button>();
                var eliteBtn = canvasObj.transform.Find("MinionPanel/EliteButton").GetComponent<Button>();

                meleeBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Melee));
                rangedBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Ranged));
                eliteBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Elite));
            }
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
        // ������ �Ϸ�� ������ ��� �� ���� ����
        StartCoroutine(WaitForRoomPropertiesAndJoin());
    }

    private IEnumerator WaitForRoomPropertiesAndJoin()
    {
        // Role�� �Ҵ�� ������ ���
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Role"))
            yield return null;
        // Team�� �Ҵ�� ������ ���
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            yield return null;

        // ������ ������ �� �÷��̾� ������Ʈ ����
        SetRole();
    }

    // ���ҿ� ���� ������Ʈ ����
    private void SetRole()
    {
        int myTeamId = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        string myRole = (string)PhotonNetwork.LocalPlayer.CustomProperties["Role"];

        if (myRole == "Hero")
        {
            // Hero�� ����
            Vector3 pos = (myTeamId == 0) ? heroRedSpawnPoint.position : heroBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? heroRedSpawnPoint.rotation : heroBlueSpawnPoint.rotation;
            PhotonNetwork.Instantiate("Hero1", pos, rot);
        }
        else if (myRole == "Command")
        {
            // 1. HQ ���� ����
            Vector3 hqPos = (myTeamId == 0) ? hqRedSpawnPoint.position : hqBlueSpawnPoint.position;
            Quaternion hqRot = (myTeamId == 0) ? hqRedSpawnPoint.rotation : hqBlueSpawnPoint.rotation;
            var hqObj = PhotonNetwork.Instantiate("HQ", hqPos, hqRot);

            // 2. CommandPlayer ���� �� Canvas ����
            Vector3 pos = (myTeamId == 0) ? cmdRedSpawnPoint.position : cmdBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? cmdRedSpawnPoint.rotation : cmdBlueSpawnPoint.rotation;
            var cmdObj = PhotonNetwork.Instantiate("CommandPlayer", pos, rot);

            var commandPlayer = cmdObj.GetComponent<CommandPlayer>();
            var hq = hqObj.GetComponent<HQCommander>();
            hq.player = commandPlayer;

            if (commandPlayer.photonView.IsMine)
            {
                var canvasObj = Instantiate(canvasPrefab);

                // --- Canvas Text ���� ---
                commandPlayer.goldText = canvasObj.transform.Find("ResourcePanel/GoldText").GetComponent<TMP_Text>();
                commandPlayer.gearText = canvasObj.transform.Find("ResourcePanel/GearText").GetComponent<TMP_Text>();
                commandPlayer.playerInputHandler = canvasObj.GetComponent<PlayerInputHandler>();

                // --- �̴Ͼ� ���� ��ư �̺�Ʈ ���� ---
                var meleeBtn = canvasObj.transform.Find("MinionPanel/MeleeButton").GetComponent<Button>();
                var rangedBtn = canvasObj.transform.Find("MinionPanel/RangedButton").GetComponent<Button>();
                var eliteBtn = canvasObj.transform.Find("MinionPanel/EliteButton").GetComponent<Button>();

                meleeBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Melee));
                rangedBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Ranged));
                eliteBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Elite));
            }
        }
    }
}