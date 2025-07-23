using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroModel : MonoBehaviour
{
    // 기본 스탯
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public int MaxHP { get; set; }
    [field: SerializeField] public int MaxMP { get; set; }
    [field: SerializeField] public float MoveSpd { get; set; }
    [field: SerializeField] public float Atk { get; set; }
    [field: SerializeField] public float AtkRange { get; set; }
    [field: SerializeField] public float Def { get; set; }

    // Observable Properties
    public ObservableProperty<int> CurHP { get; private set; } = new();
    public ObservableProperty<int> CurMP { get; private set; } = new();

    // HeroStats
    [field: SerializeField]
    private HeroStat[] HeroStats = new HeroStat[]
    {
        new("Hero1", 200, 150, 2.5f, 10f, 5f, 10f),
        new("Hero2", 250, 100, 2f, 8f, 4f, 15f),
        new("Hero3", 150, 200, 1.5f, 8f, 10f, 8f)
    };

    public void GetInitStats(int heroType)
    {
        if (heroType < 0 || heroType >= HeroStats.Length) return;

        // heroType에 따라 받아오는 HeroStat이 달라짐
        HeroStat stat = HeroStats[heroType];

        Name = stat.Name;
        MaxHP = stat.MaxHP;
        MaxMP = stat.MaxMP;
        MoveSpd = stat.MoveSpd;
        Atk = stat.Atk;
        AtkRange = stat.AtkRange;
        Def = stat.Def;
    }
}

// HeroStat 구조체
public struct HeroStat
{
    public string Name;
    public int MaxHP;
    public int MaxMP;
    public float MoveSpd;
    public float Atk;
    public float AtkRange;
    public float Def;

    public HeroStat(string name, int maxHP, int maxMP, float moveSpd, float atk, float atkRange, float def)
    {
        Name = name;
        MaxHP = maxHP;
        MaxMP = maxMP;
        MoveSpd = moveSpd;
        Atk = atk;
        AtkRange = atkRange;
        Def = def;
    }
}