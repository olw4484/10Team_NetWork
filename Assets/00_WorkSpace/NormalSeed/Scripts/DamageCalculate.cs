using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculate
{
    public static float DamageCalculation(float atkAmount, float defAmount)
    {
        float damage = atkAmount * 100 / (100 + defAmount);
        return damage;
    }
}
