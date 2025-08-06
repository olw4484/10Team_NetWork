using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface LGH_IDamagable
{
    void TakeDamage(float amount);

    void GetHeal(float amount);
}
