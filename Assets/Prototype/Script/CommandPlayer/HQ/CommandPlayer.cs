using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CommandPlayer : MonoBehaviour, IPunInstantiateMagicCallback
{
    public int teamId;
    public int gold = 150;
    public int gear = 50;
    public TMP_Text goldText, gearText;

    public PlayerInputHandler playerInputHandler;

    public PhotonView photonView;
    private Action<int, int> onResourceChangedHandler;

    private void Awake()
    {
        // �̺�Ʈ �ڵ鷯 ����
        onResourceChangedHandler = OnResourceChanged;
    }
    private void Start()
    {
        StartCoroutine(RegisterRoutine());

        if (photonView.IsMine)
        {
            // �̺�Ʈ ����
            EventManager.Instance.OnResourceChanged += onResourceChangedHandler;

            // ���� �� UI�� �� �� �����Ͽ� �ʱ� ��� ���� ǥ��
            UpdateGoldUI(gold);
            UpdateGearUI(gear); // ��� UI�� �ʱ�ȭ
        }
    }


    private IEnumerator RegisterRoutine()
    {
        while (LGH_TestGameManager.Instance == null)
        {
            yield return null;
        }
        LGH_TestGameManager.Instance.RegisterPlayer(this.gameObject);

        while (PlayerManager.Instance == null)
        {
            yield return null;
        }

        PlayerManager.Instance.AllPlayers.Add(this);
        yield break;
    }

    private void OnDisable()
    {
        if (photonView.IsMine)
        {
            EventManager.Instance.OnResourceChanged -= OnResourceChanged;
        }
    }

    private void UpdateGoldUI(int newGold)
    {
        if (goldText != null)
            goldText.text = $"Gold: {newGold}";
    }

    private void UpdateGearUI(int newGear)
    {
        if (gearText != null)
            gearText.text = $"Gear: {newGear}";
    }

    private void OnResourceChanged(int updatedTeamId, int newGold)
    {
        if (teamId == updatedTeamId)
        {
            gold = newGold;
            UpdateGoldUI(newGold);
        }
    }
    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[CommendPlayer] ��� +{amount} �� ����: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
    }

    public bool TrySpendGold(int amount)
    {
        if (gold < amount)
        {
            Debug.LogWarning($"[CommendPlayer] ��� ����: {gold}/{amount}");
            return false;
        }

        gold -= amount;
        Debug.Log($"[CommendPlayer] ��� -{amount} �� ����: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
        return true;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] data = info.photonView.InstantiationData;
        if (data != null && data.Length > 0)
        {
            teamId = (int)data[0];
            Debug.Log($"[CommandPlayer] teamId ����ȭ: {teamId}");
        }
    }

    public int GetGold() => gold;
    public int GetGar() => gear;

    [PunRPC]
    public void RpcAddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[CommendPlayer] ��� +{amount} �� ����: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
    }

    [PunRPC]
    public void RpcTrySpendGold(int amount)
    {
        gold -= amount;
        Debug.Log($"[CommendPlayer] ��� -{amount} �� ����: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
    }
}
