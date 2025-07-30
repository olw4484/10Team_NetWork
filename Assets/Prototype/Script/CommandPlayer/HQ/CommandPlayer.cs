using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class CommandPlayer : MonoBehaviour
{
    public int teamId;
    public int gold = 150;
    public int gear = 50;
    public TMP_Text goldText, gearText;
    public PhotonView photonView;
    void Update()
    {
        goldText.text = $"Gold: {gold}";
        gearText.text = $"Gear: {gear}";
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[CommendPlayer] °ñµå +{amount} ¡æ ÇöÀç: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
    }

    public bool TrySpendGold(int amount)
    {
        if (gold < amount)
        {
            Debug.LogWarning($"[CommendPlayer] °ñµå ºÎÁ·: {gold}/{amount}");
            return false;
        }

        gold -= amount;
        Debug.Log($"[CommendPlayer] °ñµå -{amount} ¡æ ÇöÀç: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
        return true;
    }

    public int GetGold() => gold;
    public int GetGar() => gear;
    
    [PunRPC]
    public void RpcAddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[CommendPlayer] °ñµå +{amount} ¡æ ÇöÀç: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
    }

    [PunRPC]
    public void RpcTrySpendGold(int amount)
    {
        gold -= amount;
        Debug.Log($"[CommendPlayer] °ñµå -{amount} ¡æ ÇöÀç: {gold}");
        EventManager.Instance.ResourceChanged(teamId, gold);
    }
}
