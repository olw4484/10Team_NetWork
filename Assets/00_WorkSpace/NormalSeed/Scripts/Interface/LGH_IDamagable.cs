using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface LGH_IDamagable
{
    void TakeDamage(int amount);

    void GetHeal(int amount);
}
