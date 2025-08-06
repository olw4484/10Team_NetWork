using Photon.Pun;
using UnityEngine;

public class HQ : MonoBehaviour, IDamageable
{
    [Header("HQ 설정 데이터")]
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

    // 데미지 처리 (공격받을 경우)
    public void TakeDamage(int damage, GameObject attacker = null)
    {
        currentHP -= damage;
        Debug.Log($"[HQ] 피격! 현재 HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die(attacker);
        }
    }

    private void Die(GameObject killer)
    {
        Debug.Log("[HQ] 파괴됨!");
        OnDestroyed();
    }

    private void OnDestroyed()
    {
        Debug.Log($"{gameObject.name} HQ 파괴됨!");

        if (PhotonNetwork.InRoom)
        {
            // 네트워크로 모두에게 HQ 파괴 알림
            photonView.RPC("RpcHQDestroyed", RpcTarget.All, teamId);
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            // 로컬 오프라인
            EventManager.Instance.HQDestroyed(teamId);
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void RpcHQDestroyed(int destroyedTeamId)
    {
        // UI/승패/게임오버 처리
        EventManager.Instance.HQDestroyed(destroyedTeamId);
    }
}
