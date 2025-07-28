using Photon.Realtime;
using TMPro;
using UnityEngine;

public class KMS_CommandPlayer : MonoBehaviour
{
    public int teamId;
    public int gold = 150;
    public int gear = 50;
    public TMP_Text goldText, gearText;
    void Update()
    {
        goldText.text = $"Gold: {gold}";
        gearText.text = $"Gear: {gear}";
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

    public int GetGold() => gold;
}
