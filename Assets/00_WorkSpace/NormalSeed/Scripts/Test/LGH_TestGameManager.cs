using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LGH_TestGameManager : MonoBehaviourPunCallbacks
{
    public static LGH_TestGameManager Instance { get; private set; }

    public CameraController camController;
    public TestSkillManager skillManager;

    public List<GameObject> playerList = new List<GameObject>();
    private void Awake()
    {
        // �̹� �ν��Ͻ��� �����ϸ� �ߺ� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ������ �� �ε� �г��� �ҷ����� ��� ���� ��Ȱ��ȭ, playerList�� ũ�Ⱑ 4�� �Ǹ�(�����ϰ� �ִ� �÷��̾� ���� ������ ����) �ε� �г��� ���� ���� Ȱ��ȭ

    private void Update()
    {
        // �׽�Ʈ�� �÷��̾� Ȱ��ȭ Ű�� ESC�� ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach(GameObject player in playerList) 
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
            // ���⼭ SetActive(false)�� �ɾ ����Ʈ���� ������ ������ �Ұ����ϰ� ����� ��
            player.SetActive(false);
            playerList.Add(player);
            Debug.Log(playerList.Count + "�� �ε� �Ϸ�");
        }
    }
    // �ε��� �Ϸ�Ǹ� SetActive(false)�� �س��� �÷��̾���� ��� SetActive(true)��

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

        camController.InitCamera(localPlayer);  // �÷��̾� ����
        skillManager.InitSkillManager(localPlayer);
    }
}
