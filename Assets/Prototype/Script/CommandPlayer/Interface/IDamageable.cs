using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    //void TakeDamage(int amount, GameObject attacker = null);
    void RPC_TakeDamage(int amount, int attackerViewID = -1);
    void TakeDamage(int amount, GameObject attacker = null);
}
