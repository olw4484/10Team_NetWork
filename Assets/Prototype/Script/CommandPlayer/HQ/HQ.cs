using Photon.Pun;
using UnityEngine;

public class HQ : MonoBehaviour
{
    [Header("HQ ���� ������")]
    public KMS_HQDataSO data;
    public int teamId;

    private int currentHP;
    private float spawnTimer;
    private PhotonView photonView;

    private void Start()
    {
        currentHP = data.maxHP;
    }

    // ������ ó�� (���ݹ��� ���)
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            OnDestroyed();
        }
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

    [PunRPC]
    public void RpcHQDestroyed(int destroyedTeamId)
    {
        // UI/����/���ӿ��� ó��
        EventManager.Instance.HQDestroyed(destroyedTeamId);
    }
}
