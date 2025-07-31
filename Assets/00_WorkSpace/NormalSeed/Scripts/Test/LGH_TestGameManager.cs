using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGH_TestGameManager : MonoBehaviourPunCallbacks
{
    private const int GAME_START_PLAYER_COUNT = 4;

    public static LGH_TestGameManager Instance { get; private set; }

    public CameraController camController;
    public TestSkillManager skillManager;

    public List<GameObject> playerList = new List<GameObject>();

    public GameObject localPlayer;

    [SerializeField] private InventoryHUDView inventoryView;
    InventoryHUDPresenter inventoryHUDPresenter;

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
        // 테스트용 : 플레이어 활성화 키를 ESC로 설정
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject player in playerList)
            {
                player.gameObject.SetActive(true);
            }
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

        // InitLocalPlayer(player); // 플레이어 한
    }
    // 로딩이 완료되면 SetActive(false)를 해놨던 플레이어들을 모두 SetActive(true)로

    public override void OnJoinedRoom()
    {
        StartCoroutine(WaitForLocalPlayer());
    }

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
}
