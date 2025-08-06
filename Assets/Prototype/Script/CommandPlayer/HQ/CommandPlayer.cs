using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.Collections;

public class CommandPlayer : MonoBehaviour
{
    public int teamId;
    public int gold = 150;
    public int gear = 50;
    public TMP_Text goldText, gearText;

    public PlayerInputHandler playerInputHandler;

    public PhotonView photonView;

    private void Start()
    {
            StartCoroutine(RegisterRoutine());
    }

    private IEnumerator RegisterRoutine()
    {
        while (LGH_TestGameManager.Instance == null)
        {
            yield return null; // GameManager가 생성되기를 기다림
        }
        LGH_TestGameManager.Instance.RegisterPlayer(this.gameObject);
        yield break;
    }

    void Update()
    {
        if (goldText != null)
            goldText.text = $"Gold: {gold}";
        if (gearText != null)
            gearText.text = $"Gear: {gear}";
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[CommendPlayer] 골드 +{amount} → 현재: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
    }

    public bool TrySpendGold(int amount)
    {
        if (gold < amount)
        {
            Debug.LogWarning($"[CommendPlayer] 골드 부족: {gold}/{amount}");
            return false;
        }

        gold -= amount;
        Debug.Log($"[CommendPlayer] 골드 -{amount} → 현재: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
        return true;
    }

    public int GetGold() => gold;
    public int GetGar() => gear;
    
    [PunRPC]
    public void RpcAddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[CommendPlayer] 골드 +{amount} → 현재: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
    }

    [PunRPC]
    public void RpcTrySpendGold(int amount)
    {
        gold -= amount;
        Debug.Log($"[CommendPlayer] 골드 -{amount} → 현재: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
    }
}
