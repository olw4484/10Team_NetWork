using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface KMS_IDamageable
{
    void TakeDamage(int amount, GameObject attacker = null);
}
