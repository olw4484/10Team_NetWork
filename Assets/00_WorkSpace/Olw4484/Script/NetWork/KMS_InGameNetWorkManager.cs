using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public bool IsDontDestroy => true;

    // ��� �帧�� �� �޼��忡�� ����
    public void StartGameAndSpawnAll(TeamSetting teamSetting)
    {
        // 1. ������ Ŭ���̾�Ʈ�� ��/���� ���� + CustomProperties ����
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 4)
        {
            int redCount = 0;
            int blueCount = 0;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                ExitGames.Client.Photon.Hashtable props = new();
                int setTeam = (redCount < 2) ? 0 : 1; // �� 2�� RED, �� 2�� BLUE
                string setJob = null;

                if (setTeam == 0)
                {
                    setJob = (redCount == 0) ? "Hero" : "Command";
                    redCount++;
                }
                else
                {
                    setJob = (blueCount == 0) ? "Hero" : "Command";
                    blueCount++;
                }

                props["Team"] = setTeam;
                props["Role"] = setJob;
                player.SetCustomProperties(props);
            }
        }

        // 2. ��� Ŭ���̾�Ʈ���� �ڽ��� ���� ��ƾ ���� (��/���� ���� �Ϸ�� ������ ���)
        StartCoroutine(SpawnRoutine());
    }

    // GemeStart �ʱ�ȭ
    private IEnumerator SpawnRoutine()
    {
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Role") ||
               !PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            yield return null;

        SpawnByRole();
    }

    private void SpawnByRole()
    {
        int myTeamId = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        string myRole = (string)PhotonNetwork.LocalPlayer.CustomProperties["Role"];
        int myHeroIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["HeroIndex"];

        if (myRole == "Hero")
        {
            string heroPrefabName = $"Hero{myHeroIndex}";
            Vector3 pos = (myTeamId == 0) ? heroRedSpawnPoint.position : heroBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? heroRedSpawnPoint.rotation : heroBlueSpawnPoint.rotation;
            PhotonNetwork.Instantiate(heroPrefabName, pos, rot);
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
        else
        {
            Debug.LogError($"SpawnByRole] �߸��� Role: {myRole}");
        }
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
        ManagerGroup.Instance.RegisterManager(this);

        Debug.Log("[InGameNetwork] Initialize ȣ���", this);

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object team) &&
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object role))
        {
            Debug.Log($"[InGameNetwork] LocalPlayer ���� Ȯ�� �� Team: {team}, Role: {role}");
        }
        else
        {
            Debug.LogWarning("[InGameNetwork] LocalPlayer�� CustomProperties�� Team �Ǵ� Role�� �����ϴ�.");
        }
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
