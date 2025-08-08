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

    // 모든 흐름을 한 메서드에서 관리
    public void ConnectGameScene()
    {
        // 1. 마스터 클라이언트만 팀/역할 배정 + CustomProperties 세팅
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

        // 2. 모든 클라이언트에서 자신의 스폰 루틴 실행 (팀/역할 세팅 완료될 때까지 대기)
        StartCoroutine(SpawnRoutine());
    }

    // GemeStart 초기화

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
            Debug.LogError("CustomProperties 중 null값 존재. 오브젝트 생성 중단.");
            yield break;
        }

        SpawnByRole();
    }

    private void SpawnByRole()
    {
        Debug.Log("SpawnByRole 진입");

        // 커스텀 프로퍼티 체크
        Debug.Log($"[SpawnByRole] Team: {PhotonNetwork.LocalPlayer.CustomProperties["Team"]}, Role: {PhotonNetwork.LocalPlayer.CustomProperties["Role"]}, HeroIndex: {PhotonNetwork.LocalPlayer.CustomProperties["HeroIndex"]}");

        int myTeamId = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        string myRole = (string)PhotonNetwork.LocalPlayer.CustomProperties["Role"];
        int myHeroIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["HeroIndex"];

        if (myRole == "Hero")
        {
            string heroPrefabName = $"Hero{myHeroIndex}";
            Debug.Log($"[SpawnByRole] Hero 스폰 시도: {heroPrefabName}");

            Vector3 pos = (myTeamId == 0) ? heroRedSpawnPoint.position : heroBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? heroRedSpawnPoint.rotation : heroBlueSpawnPoint.rotation;
            var go = PhotonNetwork.Instantiate(heroPrefabName, pos, rot, 0, new object[] { myTeamId});
            Debug.Log($"[SpawnByRole] Hero 프리팹 인스턴스 생성됨: {go}");
        }
        else if (myRole == "Command")
        {
            Vector3 hqPos = (myTeamId == 0) ? hqRedSpawnPoint.position : hqBlueSpawnPoint.position;
            Quaternion hqRot = (myTeamId == 0) ? hqRedSpawnPoint.rotation : hqBlueSpawnPoint.rotation;
            var hqObj = PhotonNetwork.Instantiate("HQ", hqPos, hqRot, 0, new object[] { myTeamId });
            Debug.Log($"[SpawnByRole] HQ 인스턴스 생성됨: {hqObj}");

            Vector3 pos = (myTeamId == 0) ? cmdRedSpawnPoint.position : cmdBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? cmdRedSpawnPoint.rotation : cmdBlueSpawnPoint.rotation;
            var cmdObj = PhotonNetwork.Instantiate("CommandPlayer", pos, rot, 0, new object[] { myTeamId });
            Debug.Log($"[SpawnByRole] CommandPlayer 인스턴스 생성됨: {cmdObj}");

            var commandPlayer = cmdObj.GetComponent<CommandPlayer>();
            if (commandPlayer == null) Debug.LogError("CommandPlayer 컴포넌트 없음");
            var hq = hqObj.GetComponent<HQCommander>();
            if (hq == null) Debug.LogError("HQCommander 컴포넌트 없음");

            Debug.Log("[SpawnByRole] BindCommandPlayer 호출");
            BindCommandPlayer(commandPlayer, hq);
        }
        else
        {
            Debug.LogError($"SpawnByRole] 잘못된 Role: {myRole}");
        }
    }

    private void BindCommandPlayer(CommandPlayer commandPlayer, HQCommander hq)
    {
        hq.player = commandPlayer;

        if (commandPlayer.photonView.IsMine)
        {
            var canvasObj = Instantiate(canvasPrefab);
            Debug.Log(canvasObj);

            // --- Canvas Text 연결 ---
            commandPlayer.goldText = canvasObj.transform.Find("ResourcePanel/GoldText").GetComponent<TMP_Text>();
            var goldTextObj = canvasObj.transform.Find("ResourcePanel/GoldText");
            if (goldTextObj == null) Debug.LogError("GoldText 경로 잘못됨");

            commandPlayer.gearText = canvasObj.transform.Find("ResourcePanel/GearText").GetComponent<TMP_Text>();
            var gearTextObj = canvasObj.transform.Find("ResourcePanel/GearText");
            if (gearTextObj == null) Debug.LogError("GearText 경로 잘못됨");

            commandPlayer.playerInputHandler = canvasObj.GetComponent<PlayerInputHandler>();
            var inputHandler = canvasObj.GetComponent<PlayerInputHandler>();
            if (inputHandler == null) Debug.LogError("PlayerInputHandler 없음");

            // --- 미니언 생성 버튼 이벤트 연결 ---
            var meleeBtnObj = canvasObj.transform.Find("UnitButtonPanel/MeleeButton");
            if (meleeBtnObj == null) Debug.LogError("MeleeButton 경로 잘못됨");
            var meleeBtn = meleeBtnObj.GetComponent<Button>();
            if (meleeBtn == null) Debug.LogError("MeleeButton에 Button 컴포넌트 없음");

            var rangedBtnObj = canvasObj.transform.Find("UnitButtonPanel/RangedButton");
            if (rangedBtnObj == null) Debug.LogError("RagnedButton 경로 잘못됨");
            var rangedBtn = rangedBtnObj.GetComponent<Button>();
            if (rangedBtn == null) Debug.LogError("RagnedButton에 Button 컴포넌트 없음");

            var eliteBtnObj = canvasObj.transform.Find("UnitButtonPanel/EliteButton");
            if (eliteBtnObj == null) Debug.LogError("EliteButton 경로 잘못됨");
            var eliteBtn = eliteBtnObj.GetComponent<Button>();
            if (eliteBtn == null) Debug.LogError("EliteButton에 Button 컴포넌트 없음");

            meleeBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Melee));
            rangedBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Ranged));
            eliteBtn.onClick.AddListener(() => hq.OnSpawnMinionButton((int)MinionType.Elite));
        }
    }

    public void Initialize()
    {
        ManagerGroup.Instance.RegisterManager(this);

        Debug.Log("[InGameNetwork] Initialize 호출됨", this);

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
