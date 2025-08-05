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

    public override void LocalInitialize(
        MinionDataSO data,
        Transform moveTarget = null,
        Transform attackTarget = null,
        WaypointGroup waypointGroup = null,
        int teamId = 0)
    {
        base.LocalInitialize(data, moveTarget, attackTarget, waypointGroup, teamId);
    }

    public override void SetManualControl(bool isManual)
    {
        // if (!canBeManuallyControlled) return; // HQȮ�� ��ɿ� �ڵ�
        this.isManual = isManual;
    }


    // ----- ���� �������̽� (ISelectable) -----
    public void Select() => view?.SetHighlight(true);
    public void Deselect() => view?.SetHighlight(false);
    public SelectableType GetSelectableType() => SelectableType.Minion;

    public override void SetSelected(bool isSelected)
    {
        if (isSelected) Select();
        else Deselect();
    }

    // ----- RPC ���� -----
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
            SetAttackTarget(targetObj.transform);
    }

    // ----- ���� ���� -----
    protected override void TryAttack()
    {
        if (!photonView.IsMine || attackTimer < attackCooldown || attackTarget == null || isDead) return;

        Debug.Log($"[Minion] TryAttack ���: {attackTarget?.name} / Tag: {attackTarget?.tag}");

        var targetPV = attackTarget.GetComponent<PhotonView>();
        if (targetPV == null)
        {
            Debug.LogError($"[Minion] TryAttack: attackTarget�� PhotonView ����! attackTarget: {attackTarget?.name}");
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
        // �߻�ȭ�� ó���ų�, Ranged/Melee���� �������̵��� �κ�
    }
}