using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static ISelectable;

public class MinionController : BaseMinionController, ISelectable
{

    private bool isManual = false;
    public override bool IsManualControlled => isManual;

    protected override void Awake() => base.Awake();
    protected override void Start() => base.Start();
    protected override void Update() => base.Update();

    public override void Initialize(MinionDataSO data, Transform target, WaypointGroup waypointGroup = null, int teamId = 0)
    {
        base.Initialize(data, target, waypointGroup, teamId);
    }

    public void SetManualControl(bool isManual)
    {
        this.isManual = isManual;
    }


    // ----- 선택 인터페이스 (ISelectable) -----
    public void Select() => view?.SetHighlight(true);
    public void Deselect() => view?.SetHighlight(false);
    public SelectableType GetSelectableType() => SelectableType.Minion;

    public override void SetSelected(bool isSelected)
    {
        if (isSelected) Select();
        else Deselect();
    }

    // ----- RPC 제어 -----
    [PunRPC]
    public void RpcMoveToPosition(Vector3 position)
    {
        if (photonView.IsMine)
            MoveToPosition(position);
    }

    [PunRPC]
    public void RpcSetAttackMoveTarget(Vector3 point)
    {
        if (photonView.IsMine)
            SetAttackMoveTarget(point);
    }

    [PunRPC]
    public void RpcSetTarget(int targetViewID)
    {
        if (!photonView.IsMine) return;

        var targetObj = PhotonView.Find(targetViewID);
        if (targetObj != null)
            SetTarget(targetObj.transform);
    }

    // ----- 공격 로직 -----
    protected override void TryAttack()
    {
        if (!photonView.IsMine) return;

        if (attackTimer >= attackCooldown && target != null)
        {
            attackTimer = 0f;
            int targetViewID = target.GetComponent<PhotonView>().ViewID;
            photonView.RPC(nameof(RPC_TryAttack), RpcTarget.All, targetViewID);
        }
    }

    [PunRPC]
    private void RPC_TryAttack(int targetViewID)
    {
        view?.PlayMinionAttackAnimation();
        // 추상화된 처리거나, Ranged/Melee에서 오버라이드할 부분
    }
}