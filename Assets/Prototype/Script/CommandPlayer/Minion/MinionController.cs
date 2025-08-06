using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static ISelectable;

public class MinionController : BaseMinionController, ISelectable
{
    public override bool IsManualControl => isManual;

    protected override void Awake() => base.Awake();
    protected override void Start() => base.Start();
    protected override void Update() => base.Update();

    public override void SetManualControl(bool isManual)
    {
        // if (!canBeManuallyControlled) return; // HQ확장 기능용 코드
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
        MoveToPosition(position);
    }

    [PunRPC]
    public void RpcSetAttackMoveTarget(Vector3 point)
    {
        SetAttackMoveTarget(point);
    }

    [PunRPC]
    public void RpcSetTarget(int targetViewID)
    {
        var targetObj = PhotonView.Find(targetViewID);
        if (targetObj != null)
            SetAttackTarget(targetObj.transform);
    }

    // ----- 공격 로직 -----
    protected override void TryAttack()
    {
        if (attackTimer < attackCooldown || attackTarget == null || isDead) return;

        Debug.Log($"[Minion] TryAttack 대상: {attackTarget?.name} / Tag: {attackTarget?.tag}");

        var targetPV = attackTarget.GetComponent<PhotonView>();
        if (targetPV == null)
        {
            Debug.LogError($"[Minion] TryAttack: attackTarget에 PhotonView 없음! attackTarget: {attackTarget?.name}");
            return;
        }

        attackTimer = 0f;
        int targetViewID = targetPV.ViewID;
        photonView.RPC(nameof(RPC_TryAttack), RpcTarget.All, targetViewID);
    }

    [PunRPC]
    private void RPC_TryAttack(int targetViewID)
    {
        view?.PlayMinionAttackAnimation();
        // 추상화된 처리거나, Ranged/Melee에서 오버라이드할 부분
    }
}