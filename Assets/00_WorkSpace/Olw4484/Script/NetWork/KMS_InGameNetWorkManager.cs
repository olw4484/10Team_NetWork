using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//public enum TestTeamSetting
//{
//    Red,
//    Blue
//}

public class KMS_InGameNetWorkManager : MonoBehaviourPunCallbacks , IManager
{

    public Transform heroRedSpawnPoint;
    public Transform heroBlueSpawnPoint;

    public Transform hqRedSpawnPoint;
    public Transform hqBlueSpawnPoint;
    public Transform cmdRedSpawnPoint;
    public Transform cmdBlueSpawnPoint;

    public GameObject canvasPrefab;

    public int Priority => (int)ManagerPriority.InGameNetworkManager;

    public bool IsDontDestroy => false;

    private void Awake()
    {
        ManagerGroup.Instance.RegisterManager(this);
    }

    // GemeStart �ʱ�ȭ
    private void StartSpawnProcess()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Role") ||
               !PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            yield return null;

        SetRole(0);
    }

    // ���ҿ� ���� ������Ʈ ����
    private void SetRoleInternal(int heroIndex)
    {
        int myTeamId = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        string myRole = (string)PhotonNetwork.LocalPlayer.CustomProperties["Role"];

        if (myRole == "Hero")
        {
            // Hero�� ����
            Vector3 pos = (myTeamId == 0) ? heroRedSpawnPoint.position : heroBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? heroRedSpawnPoint.rotation : heroBlueSpawnPoint.rotation;
            PhotonNetwork.Instantiate($"Hero{heroIndex}", pos, rot);
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
            BindCommandPlayer(commandPlayer, hq);
        }
    }

    public void SetRole(int heroIndex)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team") ||
            !PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Role"))
        {
            Debug.LogWarning("SetRole: ���̳� ���� ������ �����ϴ�. ���� ��ҵ�.");
            return;
        }

        SetRoleInternal(heroIndex);
    }

    private void BindCommandPlayer(CommandPlayer commandPlayer, HQCommander hq)
    {
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

    public void Initialize()
    {
        Debug.Log("[InGameNetwork] Initialize ȣ���", this);

        if (PhotonNetwork.InRoom)
            StartSpawnProcess();
    }

    public void Cleanup()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public GameObject GetGameObject()=> this.gameObject;
}
