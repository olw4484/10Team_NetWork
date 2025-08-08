using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KMS_InGameNetWorkManager : MonoBehaviourPunCallbacks, IManager
{
    [Header("RedSpawn")]
    public Transform heroRedSpawnPoint;
    public Transform cmdRedSpawnPoint;
    public Transform hqRedSpawnPoint;

    [Header("BlueSpawn")]
    public Transform heroBlueSpawnPoint;
    public Transform cmdBlueSpawnPoint;
    public Transform hqBlueSpawnPoint;

    [Header("CommandCanvas")]
    public GameObject canvasPrefab;

    public bool IsDontDestroy => true;

    // ��� �帧�� �� �޼��忡�� ����
    public void ConnectGameScene()
    {
        // 1. ������ Ŭ���̾�Ʈ�� ��/���� ���� + CustomProperties ����
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 4)
        {
            int redCount = 0;
            int blueCount = 0;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                ExitGames.Client.Photon.Hashtable props = new();
                string setJob = null;

                if ((TeamSetting)player.CustomProperties["Team"] == TeamSetting.Red)
                {
                    setJob = (redCount == 0) ? "Hero" : "Command";
                    redCount++;
                }
                else
                {
                    setJob = (blueCount == 0) ? "Hero" : "Command";
                    blueCount++;
                }

                props["Role"] = setJob;
                player.SetCustomProperties(props);
            }
        }

        // 2. ��� Ŭ���̾�Ʈ���� �ڽ��� ���� ��ƾ ���� (��/���� ���� �Ϸ�� ������ ���)
        StartCoroutine(SpawnRoutine());
    }

    // GemeStart �ʱ�ȭ

    public void StartSpawnProcess()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Role") ||
               !PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team") ||
               !PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("HeroIndex"))
            yield return null;

        var props = PhotonNetwork.LocalPlayer.CustomProperties;

        if (props["Role"] == null || props["Team"] == null || props["HeroIndex"] == null)
        {
            Debug.LogError("CustomProperties �� null�� ����. ������Ʈ ���� �ߴ�.");
            yield break;
        }

        SpawnByRole();
    }

    private void SpawnByRole()
    {
        Debug.Log("SpawnByRole ����");

        // Ŀ���� ������Ƽ üũ
        Debug.Log($"[SpawnByRole] Team: {PhotonNetwork.LocalPlayer.CustomProperties["Team"]}, Role: {PhotonNetwork.LocalPlayer.CustomProperties["Role"]}, HeroIndex: {PhotonNetwork.LocalPlayer.CustomProperties["HeroIndex"]}");

        int myTeamId = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        string myRole = (string)PhotonNetwork.LocalPlayer.CustomProperties["Role"];
        int myHeroIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["HeroIndex"];

        if (myRole == "Hero")
        {
            string heroPrefabName = $"Hero{myHeroIndex}";
            Debug.Log($"[SpawnByRole] Hero ���� �õ�: {heroPrefabName}");

            Vector3 pos = (myTeamId == 0) ? heroRedSpawnPoint.position : heroBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? heroRedSpawnPoint.rotation : heroBlueSpawnPoint.rotation;
            var go = PhotonNetwork.Instantiate(heroPrefabName, pos, rot, 0, new object[] { myTeamId});
            Debug.Log($"[SpawnByRole] Hero ������ �ν��Ͻ� ������: {go}");
        }
        else if (myRole == "Command")
        {
            Vector3 hqPos = (myTeamId == 0) ? hqRedSpawnPoint.position : hqBlueSpawnPoint.position;
            Quaternion hqRot = (myTeamId == 0) ? hqRedSpawnPoint.rotation : hqBlueSpawnPoint.rotation;
            var hqObj = PhotonNetwork.Instantiate("HQ", hqPos, hqRot, 0, new object[] { myTeamId });
            Debug.Log($"[SpawnByRole] HQ �ν��Ͻ� ������: {hqObj}");

            Vector3 pos = (myTeamId == 0) ? cmdRedSpawnPoint.position : cmdBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? cmdRedSpawnPoint.rotation : cmdBlueSpawnPoint.rotation;
            var cmdObj = PhotonNetwork.Instantiate("CommandPlayer", pos, rot, 0, new object[] { myTeamId });
            Debug.Log($"[SpawnByRole] CommandPlayer �ν��Ͻ� ������: {cmdObj}");

            var commandPlayer = cmdObj.GetComponent<CommandPlayer>();
            if (commandPlayer == null) Debug.LogError("CommandPlayer ������Ʈ ����");
            var hq = hqObj.GetComponent<HQCommander>();
            if (hq == null) Debug.LogError("HQCommander ������Ʈ ����");

            Debug.Log("[SpawnByRole] BindCommandPlayer ȣ��");
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
            Debug.Log(canvasObj);

            // --- Canvas Text ���� ---
            commandPlayer.goldText = canvasObj.transform.Find("ResourcePanel/GoldText").GetComponent<TMP_Text>();
            var goldTextObj = canvasObj.transform.Find("ResourcePanel/GoldText");
            if (goldTextObj == null) Debug.LogError("GoldText ��� �߸���");

            commandPlayer.gearText = canvasObj.transform.Find("ResourcePanel/GearText").GetComponent<TMP_Text>();
            var gearTextObj = canvasObj.transform.Find("ResourcePanel/GearText");
            if (gearTextObj == null) Debug.LogError("GearText ��� �߸���");

            commandPlayer.playerInputHandler = canvasObj.GetComponent<PlayerInputHandler>();
            var inputHandler = canvasObj.GetComponent<PlayerInputHandler>();
            if (inputHandler == null) Debug.LogError("PlayerInputHandler ����");

            // --- �̴Ͼ� ���� ��ư �̺�Ʈ ���� ---
            var meleeBtnObj = canvasObj.transform.Find("UnitButtonPanel/MeleeButton");
            if (meleeBtnObj == null) Debug.LogError("MeleeButton ��� �߸���");
            var meleeBtn = meleeBtnObj.GetComponent<Button>();
            if (meleeBtn == null) Debug.LogError("MeleeButton�� Button ������Ʈ ����");

            var rangedBtnObj = canvasObj.transform.Find("UnitButtonPanel/RangedButton");
            if (rangedBtnObj == null) Debug.LogError("RagnedButton ��� �߸���");
            var rangedBtn = rangedBtnObj.GetComponent<Button>();
            if (rangedBtn == null) Debug.LogError("RagnedButton�� Button ������Ʈ ����");

            var eliteBtnObj = canvasObj.transform.Find("UnitButtonPanel/EliteButton");
            if (eliteBtnObj == null) Debug.LogError("EliteButton ��� �߸���");
            var eliteBtn = eliteBtnObj.GetComponent<Button>();
            if (eliteBtn == null) Debug.LogError("EliteButton�� Button ������Ʈ ����");

            meleeBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Melee));
            rangedBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Ranged));
            eliteBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Elite));
        }
    }

    public void Initialize()
    {
        ManagerGroup.Instance.RegisterManager(this);

        Debug.Log("[InGameNetwork] Initialize ȣ���", this);

        ConnectGameScene();
    }

    public void Cleanup()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public GameObject GetGameObject() => this.gameObject;
}
