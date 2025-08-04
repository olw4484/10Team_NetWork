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

    // 모든 흐름을 한 메서드에서 관리
    public void StartGameAndSpawnAll(TeamSetting teamSetting)
    {
        // 1. 마스터 클라이언트만 팀/역할 배정 + CustomProperties 세팅
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 4)
        {
            int redCount = 0;
            int blueCount = 0;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                ExitGames.Client.Photon.Hashtable props = new();
                int setTeam = (redCount < 2) ? 0 : 1; // 앞 2명 RED, 뒤 2명 BLUE
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

        // 2. 모든 클라이언트에서 자신의 스폰 루틴 실행 (팀/역할 세팅 완료될 때까지 대기)
        StartCoroutine(SpawnRoutine());
    }

    // GemeStart 초기화
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
            // 1. HQ 먼저 생성
            Vector3 hqPos = (myTeamId == 0) ? hqRedSpawnPoint.position : hqBlueSpawnPoint.position;
            Quaternion hqRot = (myTeamId == 0) ? hqRedSpawnPoint.rotation : hqBlueSpawnPoint.rotation;
            var hqObj = PhotonNetwork.Instantiate("HQ", hqPos, hqRot);

            // 2. CommandPlayer 생성 및 Canvas 연결
            Vector3 pos = (myTeamId == 0) ? cmdRedSpawnPoint.position : cmdBlueSpawnPoint.position;
            Quaternion rot = (myTeamId == 0) ? cmdRedSpawnPoint.rotation : cmdBlueSpawnPoint.rotation;
            var cmdObj = PhotonNetwork.Instantiate("CommandPlayer", pos, rot);

            var commandPlayer = cmdObj.GetComponent<CommandPlayer>();
            var hq = hqObj.GetComponent<HQCommander>();
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

            // --- Canvas Text 연결 ---
            commandPlayer.goldText = canvasObj.transform.Find("ResourcePanel/GoldText").GetComponent<TMP_Text>();
            commandPlayer.gearText = canvasObj.transform.Find("ResourcePanel/GearText").GetComponent<TMP_Text>();
            commandPlayer.playerInputHandler = canvasObj.GetComponent<PlayerInputHandler>();

            // --- 미니언 생성 버튼 이벤트 연결 ---
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

        Debug.Log("[InGameNetwork] Initialize 호출됨", this);

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object team) &&
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Role", out object role))
        {
            Debug.Log($"[InGameNetwork] LocalPlayer 정보 확인 → Team: {team}, Role: {role}");
        }
        else
        {
            Debug.LogWarning("[InGameNetwork] LocalPlayer의 CustomProperties에 Team 또는 Role이 없습니다.");
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
