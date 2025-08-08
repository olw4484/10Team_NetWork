using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LGH_TestGameManager : MonoBehaviourPunCallbacks, IManager
{
    private const int GAME_START_PLAYER_COUNT = 4;

    public static LGH_TestGameManager Instance { get; private set; }

    public bool IsDontDestroy => false;

    public CameraController camController;
    public TestSkillManager skillManager;

    public List<GameObject> playerList = new List<GameObject>();

    public GameObject localPlayer;

    [SerializeField] private InventoryHUDView inventoryView;
    InventoryHUDPresenter inventoryHUDPresenter;

    [Header("팀별 리스폰 포인트")]
    [SerializeField] private Transform redSpawnPoint;
    [SerializeField] private Transform blueSpawnPoint;

    private float respawnDelay = 5f;

    private void Awake()
    {
        // 이미 인스턴스가 존재하면 중복 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // 시작할 때 로딩 패널을 불러오고 모든 조작 비활성화, playerList의 크기가 4가 되면(참여하고 있는 플레이어 수와 동일해 지면) 로딩 패널을 끄고 조작 활성화

    private void Update()
    {
        //테스트용: 플레이어 활성화 키를 ESC로 설정, ESC 입력시 Initialize()를 실행한 후 플레이어를 활성화시켜줌. 테스트용이니 다른 씬에서는 ESC 누르지 말아주세요
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Initialize();
            foreach (GameObject player in playerList)
            {
                player.gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log($"플레이어 정보 : {(string)PhotonNetwork.LocalPlayer.CustomProperties["Team"]}");
            Debug.Log($"플레이어 정보 : {(string)PhotonNetwork.LocalPlayer.CustomProperties["Role"]}");
        }
    }

    public void RegisterPlayer(GameObject player)
    {
        PhotonView pv = player.GetComponent<PhotonView>();
        if (pv != null && player.CompareTag("Player"))
        {
            // 여기서 SetActive(false)로 걸어서 리스트에는 넣지만 조작은 불가능하게 만들어 줌
            player.SetActive(false);
            playerList.Add(player);
            Debug.Log(playerList.Count + "명 로딩 완료");
        }

        if (pv == null)
        {
            Debug.Log($"{pv.name}포톤뷰가 없습니다.");
        }

        if (playerList.Count == GAME_START_PLAYER_COUNT)
        {
            ActivateAllPlayers();
        }
        // InitLocalPlayer(player); // 플레이어 한
    }
    // 로딩이 완료되면 SetActive(false)를 해놨던 플레이어들을 모두 SetActive(true)로
    private void ActivateAllPlayers()
    {
        foreach (GameObject player in playerList)
        {
            if (player != null)
                player.SetActive(true);
        }

        Debug.Log("모든 플레이어 활성화됨!");
    }

    //public override void OnJoinedRoom()
    //{
    //    StartCoroutine(WaitForLocalPlayer());
    //}

    private IEnumerator WaitForLocalPlayer()
    {
        GameObject localPlayer = null;
        while (localPlayer == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject obj in players)
            {
                PhotonView pv = obj.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    localPlayer = obj;
                    break;
                }
            }
            yield return null;
        }

        InitLocalPlayer(localPlayer);
    }

    private void InitLocalPlayer(GameObject gameObject)
    {
        localPlayer = gameObject;

        if (localPlayer == null) return;

        camController.InitCamera(localPlayer);  // 플레이어 전달
        skillManager.InitSkillManager(localPlayer);

        if (inventoryView != null)
        {
            InventoryHUDModel model = new(localPlayer);
            inventoryHUDPresenter = new(inventoryView, model);
            inventoryView.Open();
        }
    }

    // OnJoinedRoom에서 하던 플레이어 참조 작업을 Initalize로 이관
    public void Initialize()
    {
        StartCoroutine(WaitForLocalPlayer());
        OnWin += PanelUpdate;
    }

    public void Cleanup()
    {
        OnWin -= PanelUpdate;
        playerList.Clear();
        localPlayer = null;
        Instance = null;
    }

    public GameObject GetGameObject() => this.gameObject;

    // 영웅 리스폰 로직
    public void RequestRespawn(HeroController hero)
    {
        StartCoroutine(RespawnRoutine(hero));
    }

    private IEnumerator RespawnRoutine(HeroController hero)
    {
        Debug.Log("리스폰 대기중");
        yield return new WaitForSeconds(respawnDelay);

        // 팀 정보 가져옴
        object teamObj;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out teamObj);
        string team = teamObj?.ToString();

        // 팀별 리스폰 위치 선택
        Transform spawnPoint = GetSpawnPointByTeam(team);

        // 위치 재설정 및 활성화
        hero.pv.RPC("RPC_Respawn", RpcTarget.All, spawnPoint.position, hero.model.MaxHP);
    }

    private Transform GetSpawnPointByTeam(string team)
    {
        if (team == "0")
        {
            return redSpawnPoint;
        }

        if (team == "1")
        {
            return blueSpawnPoint;
        }

        // 팀 정보가 없을 경우 디버깅
        Debug.LogWarning("팀 정보 없음.");
        return redSpawnPoint;
    }

    public Action<bool> OnWin;
    private void PanelUpdate(bool value)
    {
        SetWin(value);
        ManagerGroup.Instance.GetManager<YSJ_SystemManager>().PauseGame();
        Debug.Log($"Lose Team : {(TeamSetting)PhotonNetwork.LocalPlayer.CustomProperties["Team"]}");
    }

    public void SetWin(bool isWin)
    {
        ExitGames.Client.Photon.Hashtable props = new();
        props["Win"] = isWin;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }


}
