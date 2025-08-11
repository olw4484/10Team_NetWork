using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class HQ : MonoBehaviourPun, IDamageable, IPunInstantiateMagicCallback, IPunObservable
{
    [Header("HQ 설정 데이터")]
    public HQDataSO data;
    public int teamId;
    public bool isDead;

    [Header("Runtime")]
    [SerializeField] private int currentHP;

    int IDamageable.teamId => teamId;
    bool IDamageable.isDead => isDead;

    private void Awake()
    {
        // photonView는 MonoBehaviourPun으로 pv 캐시 이미 있음 (this.photonView)
    }

    private void Start()
    {
        // 마스터만 진실 소유: 초기 HP 세팅
        if (PhotonNetwork.IsMasterClient)
            currentHP = data != null ? data.maxHP : 1000;
    }

    private void Update()
    {
#if UNITY_EDITOR
        // 디버그: 마스터에서만 HQ 즉시 파괴
        if (PhotonNetwork.IsMasterClient && Input.GetKeyUp(KeyCode.B))
        {
            ForceDestroyForDebug();
        }
#endif
    }

    // === 데미지 처리 (외부에서 호출) ===
    public void TakeDamage(int damage, GameObject attacker = null)
    {
        // 클라가 직접 때리지 말고, 항상 마스터가 판정 하도록 보정
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            int attackerId = -1;
            if (attacker != null)
            {
                var apv = attacker.GetComponent<PhotonView>();
                if (apv != null) attackerId = apv.ViewID;
            }
            photonView.RPC(nameof(RPC_RequestDamage), RpcTarget.MasterClient, damage, attackerId);
            return;
        }

        if (isDead) return;

        currentHP -= Mathf.Max(0, damage);
        Debug.Log($"[HQ] 피격! 현재 HP: {currentHP}");

        // HP 변경 사항은 SerializeView로 자동 동기화됨 (아래 UI 훅 참고)

        if (currentHP <= 0)
        {
            Die(attacker);
        }
    }

    // 마스터 전용: 데미지 요청 처리
    [PunRPC]
    private void RPC_RequestDamage(int amount, int attackerViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject attacker = null;
        if (attackerViewID != -1)
        {
            var attackerPV = PhotonView.Find(attackerViewID);
            if (attackerPV != null) attacker = attackerPV.gameObject;
        }
        TakeDamage(amount, attacker); // 마스터 로컬 처리
    }

    private void Die(GameObject killer)
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[HQ] 파괴됨!");
        OnDestroyed();
    }

    private void OnDestroyed()
    {
        if (!PhotonNetwork.InRoom)
        {
            EventManager.Instance?.HQDestroyed(teamId);
            Destroy(gameObject);
            return;
        }

        if (!PhotonNetwork.IsMasterClient) return; // 마스터만 파괴 선언

        Debug.Log($"{gameObject.name} HQ 파괴됨! (team:{teamId})");

        // 1) 먼저 모든 클라에 게임오버/승패 알림
        photonView.RPC(nameof(RpcHQDestroyed), RpcTarget.All, teamId);

        // 2) 네트워크 오브젝트 파괴
        PhotonNetwork.Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void ForceDestroyForDebug()
    {
        if (isDead) return;
        Debug.Log("[HQ][DEBUG] 강제 파괴");
        Die(null);
    }
#endif

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var instantiationData = info.photonView.InstantiationData;
        if (instantiationData != null && instantiationData.Length > 0)
        {
            teamId = (int)instantiationData[0];
            Debug.Log($"[HQ] teamId 동기화: {teamId}");
        }
    }

    [PunRPC]
    public void RpcHQDestroyed(int destroyedTeamId)
    {
        // UI/승패/게임오버 처리
        EventManager.Instance?.HQDestroyed(destroyedTeamId);
        // 필요하면 팀별 승패 분기:
        // ManagerGroup.Instance?.GetManager<LGH_TestGameManager>()?.OnWin?.Invoke(myTeam == 승리팀);
    }

    // ---- HP 동기화 ----
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 마스터 → 다른 클라
        {
            stream.SendNext(currentHP);
            stream.SendNext(isDead);
        }
        else
        {
            currentHP = (int)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();

            // 여기서 UI 반영 가능 (예: 체력바)
            // HQHealthUI?.Set(currentHP, data.maxHP);
        }
    }

    [PunRPC]
    public void RPC_TakeDamage(int amount, int attackerViewID = -1)
    {
        // 방에 있으면: 마스터만 실제 데미지 적용
        if (PhotonNetwork.InRoom)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                // 클라 => 마스터 승인 요청
                photonView.RPC(nameof(RPC_RequestDamage), RpcTarget.MasterClient, amount, attackerViewID);
                return;
            }

            // (여긴 마스터) 바로 로컬 적용
            GameObject attacker = null;
            if (attackerViewID != -1)
            {
                var apv = PhotonView.Find(attackerViewID);
                if (apv != null) attacker = apv.gameObject;
            }
            TakeDamage(amount, attacker);
            return;
        }

        // 오프라인 모드: 로컬 적용
        GameObject offlineAttacker = null;
        if (attackerViewID != -1)
        {
            var apv = PhotonView.Find(attackerViewID);
            if (apv != null) offlineAttacker = apv.gameObject;
        }
        TakeDamage(amount, offlineAttacker);
    }
}