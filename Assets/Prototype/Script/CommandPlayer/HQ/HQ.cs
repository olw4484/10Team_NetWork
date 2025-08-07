using Photon.Pun;
using UnityEngine;

public class HQ : MonoBehaviour, IDamageable, IPunInstantiateMagicCallback
{
    [Header("HQ ���� ������")]
    public KMS_HQDataSO data;
    public int teamId;

    private int currentHP;
    private float spawnTimer;
    private PhotonView photonView;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }


    private void Start()
    {
        currentHP = data.maxHP;
    }

    private void Update()
    {
        DestroyHQ();
    }

    private void DestroyHQ()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            OnDestroyed();
        }
    }

    // ������ ó�� (���ݹ��� ���)
    public void TakeDamage(int damage, GameObject attacker = null)
    {
        currentHP -= damage;
        Debug.Log($"[HQ] �ǰ�! ���� HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die(attacker);
        }
    }

    private void Die(GameObject killer)
    {
        Debug.Log("[HQ] �ı���!");
        OnDestroyed();
    }

    private void OnDestroyed()
    {
        Debug.Log($"{gameObject.name} HQ �ı���!");

        if (PhotonNetwork.InRoom)
        {
            // ��Ʈ��ũ�� ��ο��� HQ �ı� �˸�
            photonView.RPC("RpcHQDestroyed", RpcTarget.All, teamId);
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            // ���� ��������
            EventManager.Instance.HQDestroyed(teamId);
            Destroy(gameObject);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            teamId = (int)data[0];
            Debug.Log($"[HQ] teamId ����ȭ: {teamId}");
        }
    }

    [PunRPC]
    public void RpcHQDestroyed(int destroyedTeamId)
    {
        // UI/����/���ӿ��� ó��
        EventManager.Instance.HQDestroyed(destroyedTeamId);
    }
}
