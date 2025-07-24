using UnityEngine;

public class KMS_CommandPlayer : MonoBehaviour
{
    public int teamId;
    public int gold = 100;

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
}
