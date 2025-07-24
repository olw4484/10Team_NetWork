using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinionData", menuName = "Minion/MinionData")]
public class MinionDataSO : ScriptableObject
{
    public MinionType minionType;

    [Header("Stats")]
    public int maxHP;
    public int attackPower;
    public float moveSpeed;
    public float attackRange;
    public float attackCooldown;

    [Header("Visual")]
    public GameObject prefab;
}
